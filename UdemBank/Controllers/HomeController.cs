using Microsoft.AspNetCore.Mvc;

namespace UdemBank.Controllers
{
    public class HomeController : Controller
    {
        private readonly UdemBankContext _context;

        public HomeController(UdemBankContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["UsersCount"] = _context.Users.Count();
            ViewData["SavingGroupsCount"] = _context.SavingGroups.Count();
            ViewData["LoansCount"] = _context.Loans.Count();
            ViewData["TradesCount"] = _context.Trades.Count();
            
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
