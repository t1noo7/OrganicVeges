using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Helpper;
using DiChoSaiGon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiChoSaiGon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff", Policy = "AdminAndStaffPolicy", AuthenticationSchemes = "AdminAuthen, StaffAuthen")]
    public class AdminPermissionsController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;
        public INotyfService _notyfService { get; }

        public AdminPermissionsController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }

        public async Task<IActionResult> Index(int page = 1, int RoleID = 0)
        {
            var pageNumber = page;
            var pageSize = 20;

            List<Permission> lsPermissions = new List<Permission>();

            if (RoleID != 0)
            {
                lsPermissions = _context.Permissions
                    .Include(x => x.Role).Include(x => x.Function)
                    .AsNoTracking()
                    .Where(x => x.RoleId == RoleID)
                    .OrderByDescending(x => x.PermissionId)
                    .ToList();
            }
            else
            {
                lsPermissions = _context.Permissions
                    .Include(x => x.Role).Include(x => x.Function)
                    .AsNoTracking()
                    .OrderByDescending(x => x.PermissionId)
                    .ToList();
            }

            PagedList<Permission> models = new PagedList<Permission>(lsPermissions.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentRoleID = RoleID;
            ViewBag.CurrentPage = pageNumber;
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleName", RoleID);

            return View(models);
        }




        public IActionResult Filter(int RoleID= 0)
        {
            var url = $"/Admin/AdminPermissions?RoleId={RoleID}";
            if (RoleID == 0)
            {
                url = $"/Admin/AdminPermissions";
            }
            return Json(new { status = "success", redirectUrl = url });
        }


        public IActionResult Create()
        {
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            ViewData["Function"] = new SelectList(_context.Functions, "FunctionId", "FunctionName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PermissionId, FunctionId, CanCreate, CanRead, CanEdit, CanDelete, AccessPermission, RoleId")] Permission permission)
        {
            int roleId = permission.RoleId;
            int functionId = permission.FunctionId;
            if (PermissionExists(roleId, functionId))
            {
                ModelState.AddModelError("RoleId", "Bản ghi đã tồn tại."); // Thêm lỗi vào ModelState
                ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleName", permission.RoleId);
                ViewData["Function"] = new SelectList(_context.Functions, "FunctionId", "FunctionName", permission.FunctionId);
                _notyfService.Warning("Quyền đã tồn tại!");
                return View(permission); // Trả về view với dữ liệu nhập và thông báo lỗi
            }
            if (ModelState.IsValid)
            {
                _context.Add(permission);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleName", permission.RoleId);
            ViewData["Function"] = new SelectList(_context.Functions, "FunctionId", "FunctionName", permission.FunctionId);
            return View(permission);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var permission = await _context.Permissions.Include(p => p.Role).Include(p => p.Function).FirstOrDefaultAsync(p => p.PermissionId== id);
            if (permission == null)
            {
                return NotFound();
            }

            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            ViewData["Function"] = new SelectList(_context.Functions, "FunctionId", "FunctionName");
            return View(permission);
        }

        // POST: Admin/AdminRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PermissionId, FunctionId, CanCreate, CanRead, CanEdit, CanDelete, AccessPermission, RoleId")] Permission permission)
        {
            if (id != permission.PermissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permission);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Xử lý ngoại lệ khi xảy ra lỗi
                    _notyfService.Error("Có lỗi xảy ra khi cập nhật.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(permission);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await _context.Permissions
                .Include(p => p.Role).Include(p => p.Function)
                .FirstOrDefaultAsync(m => m.PermissionId == id);
            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa sản phẩm thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool PermissionExists(int roleId, int functionId)
        {
            return _context.Permissions.Any(e => e.RoleId == roleId && e.FunctionId == functionId);
        }

        public IActionResult _SideNavPartialView()
        {
            //var userId = User.FindFirstValue("AccountId");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                // Truy vấn cơ sở dữ liệu để lấy danh sách quyền của người dùng theo userId
                {
                    var userPermissions = _context.Permissions.Where(p => p.RoleId.ToString() == userId).ToList();
                    // Đẩy danh sách quyền và các logic kiểm tra vào partial view
                    ViewData["UserPermissions"] = userPermissions;
                    return PartialView("_SideNavPartialView");
                }
            }
            else
            {
                // Xử lý trường hợp không tìm thấy userId
                // Ví dụ: Trả về một PartialView trống
                return PartialView("_SideNavPartialView");
            }
        }

    }
}
