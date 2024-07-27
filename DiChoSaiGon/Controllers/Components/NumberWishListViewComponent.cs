using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;
using DiChoSaiGon.Extension;

namespace DiChoSaiGon.Controllers.Components
{
    public class NumberWishListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var wl = HttpContext.Session.Get<List<WishListItem>>("WishList");
            return View(wl);
        }
    }
}
