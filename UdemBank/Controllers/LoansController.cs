using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace UdemBank.Controllers
{
    public class LoansController : Controller
    {
        private readonly UdemBankContext _context;

        public LoansController(UdemBankContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Saving)
                .ThenInclude(s => s.User)
                .Include(l => l.Saving)
                .ThenInclude(s => s.SavingGroup)
                .ToListAsync();
            return View(loans);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Saving)
                .ThenInclude(s => s.User)
                .Include(l => l.Saving)
                .ThenInclude(s => s.SavingGroup)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();
            return View(loan);
        }

        public IActionResult Create()
        {
            ViewData["SavingId"] = new SelectList(_context.Savings, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SavingId,Amount,Date,Type,DueDate,CurrentBalance,Paid")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SavingId"] = new SelectList(_context.Savings, "Id", "Id", loan.SavingId);
            return View(loan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();
            ViewData["SavingId"] = new SelectList(_context.Savings, "Id", "Id", loan.SavingId);
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SavingId,Amount,Date,Type,DueDate,CurrentBalance,Paid")] Loan loan)
        {
            if (id != loan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Loans.Any(e => e.Id == loan.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SavingId"] = new SelectList(_context.Savings, "Id", "Id", loan.SavingId);
            return View(loan);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var loan = await _context.Loans
                .Include(l => l.Saving)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null) return NotFound();
            return View(loan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null) _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
