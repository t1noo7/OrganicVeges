using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Areas.Admin.ViewModel;
using DiChoSaiGon.Extension;
using DiChoSaiGon.Helpper;
using DiChoSaiGon.Models;
using DiChoSaiGon.ModelViews;
using DiChoSaiGon.Areas.Admin.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DiChoSaiGon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly DiChoSaiGonEcommerceContext _context;

        public INotyfService _notifyService { get; }
        public AdminLoginController(DiChoSaiGonEcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        [Route("admin-login.html", Name = "AdminDangNhap")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            var accountID = HttpContext.Session.GetString("AccountId");
            if (accountID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        [AllowAnonymous]
        [Route("admin-login.html", Name = "AdminDangNhap")]
        public async Task<IActionResult> Login(LoginViewModel account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(account.UserName);
                    if (!isEmail)
                    {
                        return View(account);
                    }

                    var user = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == account.UserName);

                    if (user == null)
                    {
                        return View("Login");
                    }

                    string pass = (account.Password + user.Salt.Trim()).ToMD5();
                    if (user.Password != pass)
                    {
                        _notifyService.Success("Thông tin đăng nhập chưa chính xác");
                        return View("Login");
                    }

                    // kiểm tra tài khoản có bị disable hay không 
                    if (user.Active == false)
                    {
                        return RedirectToAction("AccessDenied", "AdminLogin");
                    }

                    // lưu session makh
                    HttpContext.Session.SetString("AccountId", user.AccountId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("AccountId");

                    // Identity

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim("AccountId", user.AccountId.ToString())

                    };
                    //claims.Add(new Claim("RoleId", user.RoleId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                    var permission = await _context.Permissions.Where(p => p.RoleId == user.RoleId).OrderBy(x => x.FunctionId).ToListAsync();

                    foreach (var item in permission)
                    {
                        if (item.FunctionId == 1)
                        {
                            claims.Add(new Claim("FunctionId1", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission1", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate1", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit1", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead1", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete1", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 2)
                        {
                            claims.Add(new Claim("FunctionId2", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission2", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate2", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit2", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead2", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete2", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 3)
                        {
                            claims.Add(new Claim("FunctionId3", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission3", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate3", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit3", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead3", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete3", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 4)
                        {
                            claims.Add(new Claim("FunctionId4", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission4", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate4", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit4", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead4", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete4", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 5)
                        {
                            claims.Add(new Claim("FunctionId5", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission5", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate5", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit5", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead5", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete5", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 6)
                        {
                            claims.Add(new Claim("FunctionId6", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission6", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate6", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit6", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead6", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete6", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 7)
                        {
                            claims.Add(new Claim("FunctionId7", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission7", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate7", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit7", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead7", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete7", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 8)
                        {
                            claims.Add(new Claim("FunctionId8", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission8", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate8", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit8", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead8", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete8", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 9)
                        {
                            claims.Add(new Claim("FunctionId9", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission9", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate9", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit9", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead9", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete9", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 10)
                        {
                            claims.Add(new Claim("FunctionId10", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission10", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate10", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit10", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead10", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete10", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 11)
                        {
                            claims.Add(new Claim("FunctionId11", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission11", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate11", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit11", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead11", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete11", item.CanDelete.ToString()));
                        }
                        if (item.FunctionId == 12)
                        {
                            claims.Add(new Claim("FunctionId12", item.FunctionId.ToString()));
                            claims.Add(new Claim("AccessPermission12", item.AccessPermission.ToString()));
                            claims.Add(new Claim("CanCreate12", item.CanCreate.ToString()));
                            claims.Add(new Claim("CanEdit12", item.CanEdit.ToString()));
                            claims.Add(new Claim("CanRead12", item.CanRead.ToString()));
                            claims.Add(new Claim("CanDelete12", item.CanDelete.ToString()));
                        }
                        //AddClaimsForFunctionId(item.FunctionId, item);
                    }
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "AdminAuthen");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync("AdminAuthen", claimsPrincipal);
                    _notifyService.Success("Đăng nhập thành công ");
                    if (!string.IsNullOrEmpty(account.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch
            {
                return RedirectToAction("AccessDenied", "AdminLogin");
            }
            return View(account);
        }

        [HttpGet]
        //[Route("log-out-admin.html", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");
            return RedirectToAction("Login", "AdminLogin");
        }

        [Route("access-denied.html", Name = "AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        void AddClaimsForFunctionId(int functionId, Permission item)
        {
            var claims = new List<Claim>
    {
        new Claim($"FunctionId{functionId}", item.FunctionId.ToString()),
        new Claim($"AccessPermission{functionId}", item.AccessPermission.ToString()),
        new Claim($"CanCreate{functionId}", item.CanCreate.ToString()),
        new Claim($"CanEdit{functionId}", item.CanEdit.ToString()),
        new Claim($"CanRead{functionId}", item.CanRead.ToString()),
        new Claim($"CanDelete{functionId}", item.CanDelete.ToString())
    };
        }

    }
}