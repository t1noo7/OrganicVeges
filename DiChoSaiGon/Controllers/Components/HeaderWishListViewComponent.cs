using DiChoSaiGon.Extension;
using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace DiChoSaiGon.Controllers.Components
{
    public class HeaderWishListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var wl = HttpContext.Session.Get<List<WishListItem>>("WishList");
            return View(wl);
        }
    }
}
