using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;

namespace DiChoSaiGon.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Staff", Policy = "AdminAndStaffPolicy", AuthenticationSchemes = "AdminAuthen, StaffAuthen")]
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public SearchController(DiChoSaiGonEcommerceContext context)
        {
            _context = context;
        }
        // GET: Search/FindProduct
        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                ls = _context.Products
                    .AsNoTracking()
                    .Include(a => a.Cat)
                    .Where(x => true)
                    .OrderByDescending(x => x.ProductId)
                    .ToList();

                return PartialView("AdminListProductsSearchPartial", ls);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword) || x.Cat.CatName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("AdminListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("AdminListProductsSearchPartial", ls);
            }
        }
    }
}

