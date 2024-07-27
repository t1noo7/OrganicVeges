using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiChoSaiGon.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Staff", Policy = "AdminAndStaffPolicy", AuthenticationSchemes = "AdminAuthen, StaffAuthen")]
    [Area("Admin")]

    public class HomeController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public INotyfService _notyfService { get; }

        public HomeController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }


        [Route("/admin", Name = "admin")]
        public IActionResult Index()
        {
            var recentOrders = _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Include(x => x.Customer)
            .Include(o => o.TransactStatus)
            .Take(5)
            .ToList();
            var productList = _context.Products
            .OrderBy(p => p.DateCreated)
            .Include(p => p.Cat)
            .Take(5)
            .ToList();

            int numberOfOrders = _context.Orders.Count();
            int numberOfProducts = _context.Products.Count();
            int numberOfCustomers = _context.Customers.Count();
            int? totalOrderAmount = _context.OrderDetails.Sum(od => od.TotalMoney);

            ViewBag.RecentOrders = recentOrders;
            ViewBag.NumberOfOrders = numberOfOrders;
            ViewBag.NumberOfProducts = numberOfProducts;
            ViewBag.NumberOfCustomers = numberOfCustomers;
            ViewBag.TotalOrderMoney = totalOrderAmount;
            ViewBag.ListProduct = productList;
            return View();
        }
    }
}
