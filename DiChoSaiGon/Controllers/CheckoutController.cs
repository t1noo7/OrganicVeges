
using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Extension;
using DiChoSaiGon.Helpper;
using DiChoSaiGon.Models;
using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DiChoSaiGon.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;
        public INotyfService _notifyService { get; }

        public CheckoutController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        // GET Checkout/Index
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            // lấy giỏ hàng ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            // lấy giỏ hàng ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;

                khachhang.LocationId = muaHang.TinhThanh;
                khachhang.District = muaHang.QuanHuyen;
                khachhang.Ward = muaHang.PhuongXa;
                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();

            }
            try
            {
                if (ModelState.IsValid)
                {
                    // Khởi tạo đơn hàng
                    Order donhang = new Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;
                    donhang.LocationId = muaHang.TinhThanh;
                    donhang.District = muaHang.QuanHuyen;
                    donhang.Ward = muaHang.PhuongXa;

                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1; // đơn hàng mới
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));

                    _context.Add(donhang);
                    _context.SaveChanges();

                    // Tạo danh sách đơn hàng

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Amount = item.amount;
                        orderDetail.TotalMoney = donhang.TotalMoney;
                        orderDetail.Price = item.product.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);
                    }
                    _context.SaveChanges();
                    // Clear giỏ hàng
                    HttpContext.Session.Remove("GioHang");
                    // Xuất thông báo
                    _notifyService.Success("Đặt hàng thành công");

                    // Cập nhật thông tin khách hàng
                    return RedirectToAction("Success");
                }
            }
            catch
            {
                ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }

        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success(MuaHangVM muaHang)
        {
            //string ValueTinhThanh = form["TinhThanh"].ToString();
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders.Where(x => x.CustomerId == Convert.ToInt32(taikhoanID)).OrderByDescending(x => x.OrderDate).FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.DonHangID = donhang.OrderId;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;
                //successVM.PhuongXa = Convert.ToString(donhang.Ward);
                //successVM.QuanHuyen = Convert.ToString(donhang.District);
                //successVM.TinhThanh = Convert.ToString(donhang.LocationId);
                successVM.PhuongXa = GetNameLocation(donhang.Ward.Value);
                successVM.QuanHuyen = GetNameLocation(donhang.District.Value);
                successVM.TinhThanh = GetNameLocation(donhang.LocationId.Value);
                return View(successVM);
            }
            catch
            {
                return View();
            }

        }

        public string GetNameLocation(int idLocation)
        {
            try
            {
                var location = _context.Locations.AsNoTracking().SingleOrDefault(x => x.LocationId == idLocation);
                if (location != null)
                {
                    return location.NameWithType;
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
