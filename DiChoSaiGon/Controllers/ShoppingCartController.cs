using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Extension;
using DiChoSaiGon.Models;
using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.ComponentModel;

namespace DiChoSaiGon.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public INotyfService _notifyService { get; }
        public ShoppingCartController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        // Khởi tạo giỏ hàng
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

        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                //Thêm sản phẩm vào giỏ hàng
                CartItem item = GioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)//đã có --) cập nhật số lượng
                {
                    if (amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    else
                    {
                        item.amount++;
                    }
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh
                    };
                    gioHang.Add(item);//thêm vào giỏ
                }
                //lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notifyService.Success("Thêm sản phẩm thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }



        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            // lấy giỏ hàng ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && amount.HasValue)//đã có --) cập nhật số lượng
                    {
                        item.amount = amount.Value;
                    }
                    //lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                // Lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
