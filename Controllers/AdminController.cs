using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TatryExplorer.Data;
using TatryExplorer.ViewModels;

namespace TatryExplorer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<AdminViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new AdminViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                UserId = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                TempData["ErrorMessage"] = "Podany adres email jest już używany przez inne konto.";
                return RedirectToAction(nameof(Edit), new { id = model.UserId });
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Nie możesz usunąć swojego własnego konta.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new DeleteUserViewModel
            {
                UserId = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Nie możesz usunąć swojego własnego konta.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("DeleteConfirmed method called for user ID '{UserId}'", id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Attempt to delete non-existent user with ID '{UserId}'.", id);
                return NotFound();
            }

            // Usuń powiązane trasy użytkownika
            var trails = _context.Trails.Where(t => t.UserId == user.Id).ToList();
            _context.Trails.RemoveRange(trails);
            await _context.SaveChangesAsync();

            // Usuń użytkownika
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' deleted successfully.", user.Id);
                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogError("Failed to delete user with ID '{UserId}'. Errors: {Errors}", user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var model = new DeleteUserViewModel
            {
                UserId = user.Id,
                Email = user.Email
            };

            return View("Delete", model);
        }

        public async Task<IActionResult> ManageRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                AvailableRoles = allRoles.Select(role => new RoleSelectionViewModel
                {
                    RoleName = role.Name,
                    Selected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.AvailableRoles.Where(r => r.Selected).Select(r => r.RoleName).ToList();

            if (user.Id == _userManager.GetUserId(User) && !selectedRoles.Contains("Administrator"))
            {
                TempData["ErrorMessage"] = "Nie możesz odebrać samemu sobie roli Administrator";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Nie udało się nadać roli.");
                return View(model);
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Nie udało się odebrać roli.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChangePassword(string id)
        {
            var model = new ChangePasswordViewModel { UserId = id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Nie udało się usunąć starego hasła.";
                return RedirectToAction(nameof(ChangePassword), new { id = model.UserId });
            }

            result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
                return RedirectToAction(nameof(ChangePassword), new { id = model.UserId });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
