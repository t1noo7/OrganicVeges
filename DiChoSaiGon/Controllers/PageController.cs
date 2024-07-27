using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace DiChoSaiGon.Controllers
{
    public class PageController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public PageController(DiChoSaiGonEcommerceContext context)
        {
            _context = context;
        }
        // GET: Pages/Alias
        [Route("/page/{Alias}", Name = "PagesDetails")]

        public IActionResult Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Home", "Index");
            var page = _context.Pages.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
            if (page == null)
            {
                RedirectToAction("Index", "Home");
            }
            return View(page);
        }
    }
}
