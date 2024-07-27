using AspNetCoreHero.ToastNotification.Abstractions;
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
    public class AdminOrdersController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;
        public INotyfService _notyfService { get; }
        public AdminOrdersController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }
        // GET: Admin/AdminOrders
        public IActionResult Index (int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var lsOrders = _context.Orders.Include(o => o.Customer).Include(o => o.TransactStatus).AsNoTracking().OrderByDescending(x => x.OrderDate);

            PagedList<Order> models = new PagedList<Order>(lsOrders, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders
                .AsNoTracking()
                .Include(x=>x.Customer)
                .FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,Address,PaymentDate,PaymentId,LocationId,District,Ward,TotalMoney")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.Orders.AsNoTracking().Include(x=>x.Customer).FirstOrDefaultAsync(x => x.OrderId == id);
                    if (donhang != null)
                    {
                        donhang.Paid = order.Paid;
                        donhang.Deleted = order.Deleted;
                        donhang.TransactStatusId = order.TransactStatusId;
                        if (donhang.Paid == true)
                        {
                            donhang.PaymentDate = DateTime.Now;
                        }
                        if(donhang.TransactStatusId==5) donhang.Deleted=true;
                        if(donhang.TransactStatusId==3) donhang.ShipDate=DateTime.Now;
                    }   
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thái đơn hàng thành công");
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!OrderExist(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return PartialView("ChangeStatus", order);
        }

        private bool OrderExist(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        public string GetNameLocation(int idLocation)
        {
            try
            {
                var location = _context.Locations.AsNoTracking().SingleOrDefault(x => x.LocationId == idLocation);
                if (location != null)
                {
                    return location.NameWithType;
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }
        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var Chitietdonhang = _context.OrderDetails
                .AsNoTracking()
                .Include(x=> x.Product)
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailsId)
                .ToList();

            ViewBag.Chitiet = Chitietdonhang;
            string phuongxa = GetNameLocation(order.Ward.Value);
            string quanhuyen = GetNameLocation(order.District.Value);
            string tinhthanh = GetNameLocation(order.LocationId.Value);
            string fulladdress = $"{order.Address}, {phuongxa}, {quanhuyen}, {tinhthanh}";
            ViewBag.Fulladdress = fulladdress ;
            return View(order);
        }

        // GET: Admin/AdminOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var Chitietdonhang = _context.OrderDetails
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailsId)
                .ToList();

            ViewBag.Chitiet = Chitietdonhang;
            string phuongxa = GetNameLocation(order.Ward.Value);
            string quanhuyen = GetNameLocation(order.District.Value);
            string tinhthanh = GetNameLocation(order.LocationId.Value);
            string fulladdress = $"{order.Address}, {phuongxa}, {quanhuyen}, {tinhthanh}";
            ViewBag.Fulladdress = fulladdress;
            return View(order);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Deleted = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa đơn hàng thành công");
            return RedirectToAction(nameof(Index));
        }

    }
}

