using AWEElectronics.Data;
using AWEElectronics.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AWEElectronics.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Reports/Order
        public IActionResult Order()
        {
            var model = new OrderReportViewModel();
            return View(model);
        }

        // POST: /Reports/Order
        [HttpPost]
        public async Task<IActionResult> Order(OrderReportViewModel model, string period)
        {
            DateTime fromDate;
            DateTime toDate = DateTime.Today;

            switch (period)
            {
                case "daily":
                    fromDate = DateTime.Today;
                    toDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
                case "monthly":
                    fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    toDate = fromDate.AddMonths(1).AddTicks(-1);
                    break;
                case "yearly":
                    fromDate = new DateTime(DateTime.Today.Year, 1, 1);
                    toDate = fromDate.AddYears(1).AddTicks(-1);
                    break;
                default:
                    fromDate = model.FromDate ?? DateTime.Today.AddDays(-7);
                    toDate = model.ToDate ?? DateTime.Today;
                    break;
            }

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)  // Include customer to show name
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .ToListAsync();

            model.Orders = orders;
            model.FromDate = fromDate;
            model.ToDate = toDate;

            return View(model);
        }
    }
}
