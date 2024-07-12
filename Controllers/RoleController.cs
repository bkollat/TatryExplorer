using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TatryExplorer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is required");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (roleResult.Succeeded)
                {
                    return Ok($"Role {roleName} created successfully");
                }
                else
                {
                    return BadRequest("Error while creating role");
                }
            }

            return BadRequest("Role already exists");
        }

        [HttpPost("AssignRole")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (roleResult.Succeeded)
            {
                return Ok($"Role {roleName} assigned to user {email} successfully");
            }
            else
            {
                return BadRequest("Error while assigning role");
            }
        }
    }
}
