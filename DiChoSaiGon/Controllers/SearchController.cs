using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace DiChoSaiGon.Controllers
{
    public class SearchController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public SearchController(DiChoSaiGonEcommerceContext context)
        {
            _context = context;
        }
        // GET: Search/FindProduct
        [Route("search.html", Name = "Search")]
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsSearch = _context.Products
                .AsNoTracking()
                .Include(a=>a.Cat)
                .OrderByDescending(x => x.ProductId);
            PagedList<Product> models = new PagedList<Product>(lsSearch, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> FindProduct(string keyword, int? page)
        {
            var pageNumber = page <= 0 ? 1 : page ?? 1;
            var pageSize = 10;

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductSearchPartial", null);
            }

            var lsSearch = await _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword) || x.Cat.CatName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .ToListAsync();

            var ls = new StaticPagedList<Product>(lsSearch.Skip((pageNumber - 1) * pageSize).Take(pageSize), pageNumber, pageSize, lsSearch.Count);
            //var ls = new PagedList<Product>(lsSearch, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return PartialView("ListProductSearchPartial", ls);
        }
    }
}
