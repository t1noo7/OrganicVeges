using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Helpper;
using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace DiChoSaiGon.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Staff", Policy = "AdminAndStaffPolicy", AuthenticationSchemes = "AdminAuthen, StaffAuthen")]
    [Area("Admin")]
    public class AdminBannersController : Controller
    {

        private readonly DiChoSaiGonEcommerceContext _context;

        public INotyfService _notyfService { get; }

        public AdminBannersController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var lsBanners = _context.Banners
                .AsNoTracking()
                .OrderByDescending(x => x.BannerId);
            PagedList<Banner> models = new PagedList<Banner>(lsBanners, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.BannerId == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // GET: Admin/AdminBanners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminBanners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BannerId, BannerName, Thumb, DateModified, Active, BannerHeaderText, BannerText, ActiveButton, OrderIndex")] Banner banner, Microsoft.AspNetCore.Http.IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                banner.BannerName = Utilities.ToTitleCase(banner.BannerName);
                {
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(banner.BannerName) + extension;
                        banner.Thumb = await Utilities.UploadFile(fThumb, @"banners", image.ToLower());
                    }
                }
                if (string.IsNullOrEmpty(banner.Thumb)) banner.Thumb = "default.jpg";

                banner.DateModified = DateTime.Now;
                int maxOrderIndex = await _context.Banners.MaxAsync(b => b.OrderIndex);
                banner.OrderIndex = maxOrderIndex + 1;

                _context.Add(banner);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Admin/AdminBanners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            ViewBag.OldOrderIndex = banner.OrderIndex;
            return View(banner);
        }

        // POST: Admin/AdminBanners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BannerId, BannerName, Thumb, DateModified, Active, BannerHeaderText, BannerText, ActiveButton, OrderIndex")] Banner banner, Microsoft.AspNetCore.Http.IFormFile? fThumb)
        {
            if (id != banner.BannerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    {
                        if (fThumb != null)
                        {
                            string extension = Path.GetExtension(fThumb.FileName);
                            string image = Utilities.SEOUrl(banner.BannerName) + extension;
                            banner.Thumb = await Utilities.UploadFile(fThumb, @"banners", image.ToLower());
                        }
                    }
                    if (string.IsNullOrEmpty(banner.Thumb)) banner.Thumb = "default.jpg";

                    banner.DateModified = DateTime.Now;

                    // Xác định OrderIndex mới của banner
                    int newOrderIndex = banner.OrderIndex;

                    // Kiểm tra xem có bản ghi khác có OrderIndex trùng với OrderIndex mới không
                    var otherBanner = await _context.Banners.FirstOrDefaultAsync(b => b.BannerId != banner.BannerId && b.OrderIndex == newOrderIndex);
                    if (otherBanner != null)
                    {
                        // Nếu có, swap giá trị OrderIndex của hai bản ghi
                        int oldOrderIndex = Convert.ToInt32(Request.Form["oldOrderIndex"]);
                        otherBanner.OrderIndex = oldOrderIndex;

                        // Cập nhật bản ghi vào cơ sở dữ liệu
                        _context.Update(otherBanner);
                    }

                    _context.Update(banner);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(banner.BannerId))
                    {
                        _notyfService.Warning("Có lỗi xảy ra");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Admin/AdminBanner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.BannerId == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // POST: Admin/AdminBanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner
                = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa sản phẩm thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.BannerId == id);
        }
    }
}
