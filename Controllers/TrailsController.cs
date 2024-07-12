using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatryExplorer.Data;
using TatryExplorer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace TatryExplorer.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TrailsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var trails = await _context.Trails.Where(t => t.UserId == userId).ToListAsync();
            return View(trails);
        }

        public async Task<IActionResult> AllTrails()
        {
            var userId = GetUserId();
            var trails = await _context.Trails.Include(t => t.User).Include(t => t.FavoriteTrails).ToListAsync();
            var favoriteTrailIds = await _context.FavoriteTrails.Where(ft => ft.UserId == userId).Select(ft => ft.TrailId).ToListAsync();

            var model = trails.Select(t => new TrailViewModel
            {
                Trail = t,
                IsFavorite = favoriteTrailIds.Contains(t.Id),
                FavoriteCount = t.FavoriteTrails.Count
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trail = await _context.Trails
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trail == null)
            {
                return NotFound();
            }

            // Zwiększamy licznik odwiedzin
            trail.VisitCount++;
            _context.Update(trail);
            await _context.SaveChangesAsync();

            return View(trail);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trail trail)
        {
            if (ModelState.IsValid)
            {
                trail.UserId = GetUserId();
                _context.Add(trail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trail);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trail = await _context.Trails.Include(t => t.User).FirstOrDefaultAsync(m => m.Id == id);
            if (trail == null || (trail.UserId != GetUserId() && !User.IsInRole("Administrator")))
            {
                return NotFound();
            }
            return View(trail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trail trail)
        {
            if (id != trail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!User.IsInRole("Administrator"))
                    {
                        trail.UserId = GetUserId();
                    }
                    else
                    {
                        // Ensure UserId is not modified for admin edits
                        var existingTrail = await _context.Trails.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                        if (existingTrail != null)
                        {
                            trail.UserId = existingTrail.UserId;
                        }
                    }

                    _context.Update(trail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrailExists(trail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trail);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trail = await _context.Trails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trail == null || (trail.UserId != GetUserId() && !User.IsInRole("Administrator")))
            {
                return NotFound();
            }

            return View(trail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trail = await _context.Trails.FindAsync(id);
            if (trail != null && (trail.UserId == GetUserId() || User.IsInRole("Administrator")))
            {
                _context.Trails.Remove(trail);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AllTrails)); // Change this to redirect to AllTrails
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var userId = GetUserId();
            var favoriteTrail = new FavoriteTrail { TrailId = id, UserId = userId };

            _context.FavoriteTrails.Add(favoriteTrail);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllTrails));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            var userId = GetUserId();
            var favoriteTrail = await _context.FavoriteTrails.FirstOrDefaultAsync(ft => ft.TrailId == id && ft.UserId == userId);

            if (favoriteTrail != null)
            {
                _context.FavoriteTrails.Remove(favoriteTrail);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AllTrails));
        }

        public async Task<IActionResult> Favorites()
        {
            var userId = GetUserId();
            var favoriteTrails = await _context.FavoriteTrails
                .Where(ft => ft.UserId == userId)
                .Include(ft => ft.Trail)
                .ThenInclude(t => t.User)
                .ToListAsync();

            return View(favoriteTrails.Select(ft => ft.Trail).ToList());
        }

        private bool TrailExists(int id)
        {
            return _context.Trails.Any(e => e.Id == id);
        }
    }
}
