
using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Models;
using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiChoSaiGon.Controllers
{
    public class DonHangController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;
        public INotyfService _notifyService { get; }
        
        public DonHangController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        //GET: DonHang/Details/5
        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=>x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang == null) return NotFound();
                var donhang = await _context.Orders.Include(x=>x.TransactStatus).FirstOrDefaultAsync(m=>m.OrderId == id && Convert.ToInt32(taikhoanID) == m.CustomerId);
                if (donhang == null) return NotFound();

                var chitietdonhang = _context.OrderDetails
                    .Include(x => x.Product)
                    .AsNoTracking()
                    .Where(x => x.OrderId == id)
                    .OrderBy(x => x.OrderDetailsId)
                    .ToList();
                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = chitietdonhang;

                return PartialView("Details", donHang);
            }
            catch
            {
                return NotFound();
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
