using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace UdemBank.Controllers
{
    public class TradesController : Controller
    {
        private readonly UdemBankContext _context;

        public TradesController(UdemBankContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trades = await _context.Trades
                .Include(t => t.Saving)
                .ThenInclude(s => s.User)
                .Include(t => t.Saving)
                .ThenInclude(s => s.SavingGroup)
                .ToListAsync();
            return View(trades);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trade = await _context.Trades
                .Include(t => t.Saving)
                .ThenInclude(s => s.User)
                .Include(t => t.Saving)
                .ThenInclude(s => s.SavingGroup)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trade == null) return NotFound();
            return View(trade);
        }

        public IActionResult Create()
        {
            ViewData["SavingId"] = new SelectList(_context.Savings, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SavingId,Amount,Type,Date,CurrentBalance")] Trade trade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SavingId"] = new SelectList(_context.Savings, "Id", "Id", trade.SavingId);
            return View(trade);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var trade = await _context.Trades
                .Include(t => t.Saving)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trade == null) return NotFound();
            return View(trade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade != null) _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
