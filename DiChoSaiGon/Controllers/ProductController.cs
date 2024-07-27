using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Printing;

namespace DiChoSaiGon.Controllers
{
    public class ProductController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public ProductController(DiChoSaiGonEcommerceContext context)
        {
            _context = context;
        }
        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 12;
                var lsCatNames = _context.Categories.ToList();
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, pageNumber, pageSize);

                ViewBag.CurrentPage = pageNumber;
                ViewBag.CatNames = lsCatNames;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [Route("/{Alias}", Name = "ListProduct")]

        public IActionResult List(string Alias, int page = 1)
        {
            try
            {
                var pageSize = 12;
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x=> x.Alias == Alias);
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhmuc.CatId)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, page, pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    RedirectToAction("Index");
                }

                var lsProducts = _context.Products.AsNoTracking()
                    .Where(x=>x.CatId==product.CatId && x.ProductId!= id && x.Active == true)
                    .Take(4)
                    .OrderByDescending(x=>x.DateCreated)
                    .ToList();

                ViewBag.SanPham = lsProducts;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpPost]
        public IActionResult Sort(int sortBy)
        {
            List<Product> sortedProducts = new List<Product>();
            switch (sortBy)
            {
                case 1: // Sort by Default
                    sortedProducts = _context.Products.OrderByDescending(p => p.DateCreated).ToList();
                    break;
                case 2: // Sort by name A->Z
                    sortedProducts = _context.Products.OrderBy(p => p.ProductName).ToList();
                    break;
                case 3: // Sort by name Z->A
                    sortedProducts = _context.Products.OrderByDescending(p => p.ProductName).ToList();
                    break;
                case 4: // Sort by Latest
                    sortedProducts = _context.Products.OrderBy(p => p.DateCreated).ToList();
                    break;
                case 5: // Sort by High Price
                    sortedProducts = _context.Products.OrderByDescending(p => p.Price).ToList();
                    break;
                case 6: // Sort by Low Price
                    sortedProducts = _context.Products.OrderBy(p => p.Price).ToList();
                    break;
                default:
                    sortedProducts = _context.Products.ToList(); // Default sorting
                    break;
            }

            return PartialView("ListProductPartial", sortedProducts);
        }

    }
}
