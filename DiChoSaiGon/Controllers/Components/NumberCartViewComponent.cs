using DiChoSaiGon.Extension;
using DiChoSaiGon.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace DiChoSaiGon.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
