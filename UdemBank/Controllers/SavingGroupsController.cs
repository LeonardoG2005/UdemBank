using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace UdemBank.Controllers
{
    public class SavingGroupsController : Controller
    {
        private readonly UdemBankContext _context;

        public SavingGroupsController(UdemBankContext context)
        {
            _context = context;
        }

        // GET: SavingGroups
        public async Task<IActionResult> Index()
        {
            var savingGroups = await _context.SavingGroups
                .Include(s => s.User)
                .ToListAsync();
            return View(savingGroups);
        }

        // GET: SavingGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingGroup = await _context.SavingGroups
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (savingGroup == null)
            {
                return NotFound();
            }

            // Obtener miembros del grupo
            var members = await _context.Savings
                .Include(s => s.User)
                .Where(s => s.SavingGroupId == id)
                .ToListAsync();

            ViewBag.Members = members;

            return View(savingGroup);
        }

        // GET: SavingGroups/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: SavingGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,TotalAmount")] SavingGroup savingGroup)
        {
            if (ModelState.IsValid)
            {
                // Validar que el usuario no tenga más de 3 grupos
                var userGroupsCount = await _context.Savings
                    .Where(s => s.UserId == savingGroup.UserId)
                    .Select(s => s.SavingGroupId)
                    .Distinct()
                    .CountAsync();

                if (userGroupsCount >= 3)
                {
                    ModelState.AddModelError("", "El usuario ya pertenece a 3 grupos de ahorro y no puede crear más.");
                    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", savingGroup.UserId);
                    return View(savingGroup);
                }

                _context.Add(savingGroup);
                await _context.SaveChangesAsync();

                // Crear el Saving para el creador del grupo
                var saving = new Saving
                {
                    UserId = savingGroup.UserId,
                    SavingGroupId = savingGroup.Id,
                    Affiliation = true,
                    Investment = 0
                };
                _context.Savings.Add(saving);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", savingGroup.UserId);
            return View(savingGroup);
        }

        // GET: SavingGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingGroup = await _context.SavingGroups.FindAsync(id);
            if (savingGroup == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", savingGroup.UserId);
            return View(savingGroup);
        }

        // POST: SavingGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,TotalAmount")] SavingGroup savingGroup)
        {
            if (id != savingGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savingGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingGroupExists(savingGroup.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", savingGroup.UserId);
            return View(savingGroup);
        }

        // GET: SavingGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingGroup = await _context.SavingGroups
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savingGroup == null)
            {
                return NotFound();
            }

            return View(savingGroup);
        }

        // POST: SavingGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var savingGroup = await _context.SavingGroups.FindAsync(id);
            if (savingGroup != null)
            {
                _context.SavingGroups.Remove(savingGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingGroupExists(int id)
        {
            return _context.SavingGroups.Any(e => e.Id == id);
        }
    }
}
