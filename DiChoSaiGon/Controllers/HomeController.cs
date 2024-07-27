using DiChoSaiGon.Models;
using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace DiChoSaiGon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DiChoSaiGonEcommerceContext _context;

        public HomeController(ILogger<HomeController> logger, DiChoSaiGonEcommerceContext context)
        {
            _logger = logger;
            _context = context;
        }



        public IActionResult Index()
        {
            HomeViewVM model = new HomeViewVM();

            var lsProducts = _context.Products.AsNoTracking()
                .Where(x => x.Active == true && x.HomeFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .ToList();

            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();

            var lsCats = _context.Categories.AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();

            foreach (var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProducts = lsProducts.Where(x => x.CatId == item.CatId).ToList();
                lsProductViews.Add(productHome);
            }

            var Post = _context.Posts.AsNoTracking()
                .Where(x=> x.Published == true && x.IsNewFeed == true)
                .OrderByDescending(x=> x.CreateDate)
                .Take(3)
                .ToList();

            var Banner = _context.Banners.AsNoTracking()
                .Where(x => x.Active == true)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            model.Products = lsProductViews;
            model.Posts = Post;
            model.Banners = Banner;
            ViewBag.AllProducts = lsProducts;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("gioi-thieu.html", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("lien-he.html", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
