using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace DiChoSaiGon.Controllers
{
    public class WishListController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;
        public INotyfService _notyfService { get; }
        public WishListController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }

        [Route("/wishlist.html", Name = "Wishlist")]
        public IActionResult Index()
        {
            var accountID = HttpContext.Session.GetString("CustomerId");
            if (accountID != null)
            {
                var customer = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                if (customer != null)
                {
                    var wishList = _context.WishLists
                        .Include(x => x.Product)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == customer.CustomerId)
                        .OrderByDescending(x => x.WishListId)
                        .ToList();
                    ViewBag.WishList = wishList;
                    return View(customer);
                }
            }
            return RedirectToAction("Login", "Accounts");
        }

        [HttpPost]
        public IActionResult AddToWishlist(int productID)
        {
            try
            {
                var accountID = HttpContext.Session.GetString("CustomerId");
                if (accountID != null)
                {
                    var wishList = new WishList();
                    {
                        wishList.CustomerId = Convert.ToInt32(accountID);
                        wishList.ProductId = productID;
                    }

                    _context.WishLists.Add(wishList);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm sản phẩm vào WishList thành công";
                }
                else
                {
                    return RedirectToAction("Login", "Accounts");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("WishList/remove")]
        public async Task<IActionResult> Delete(int productID)
        {
            var accountID = HttpContext.Session.GetString("CustomerId");
            if (accountID == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var wishListItem = await _context.WishLists
                .FirstOrDefaultAsync(x => x.CustomerId == Convert.ToInt32(accountID) && x.ProductId == productID);

            if (wishListItem == null)
            {
                return NotFound(); // Return NotFound if the item is not found in the wishlist
            }

            _context.WishLists.Remove(wishListItem);
            await _context.SaveChangesAsync();

            _notyfService.Success("Product removed from wishlist successfully!");
            return Json(new { success = true });
        }

    }
}
