using AspNetCoreHero.ToastNotification;
using DiChoSaiGon.Models;
using DiChoSaiGon.Models.Momo;
using DiChoSaiGon.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
builder.Services.AddSession();
var stringConnectdb = builder.Configuration.GetConnectionString("dbDiChoSaiGon");
builder.Services.AddDbContext<DiChoSaiGonEcommerceContext>(options => options.UseSqlServer(stringConnectdb));
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
//builder.Services.AddIdentity<AppUsers, IdentityRole>().AddUserStore<AppIdentityDbContext>().AddDefaultTokenProviders();
//builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
//builder.Services.AddScoped<IMomoService, MomoService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // Default scheme for users
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(p => // User login
{
    p.LoginPath = "/dang-nhap.html";
    p.AccessDeniedPath = "/";
})
.AddCookie("AdminAuthen", p =>  // Admin login
{
    p.LoginPath = "/admin-login.html";
    p.AccessDeniedPath = "/admin";
})
.AddCookie("StaffAuthen", p =>  // Staff login
{
    p.LoginPath = "/admin-login.html";
    p.AccessDeniedPath = "/access-denied.html";
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = "334474192927-e7e52a53nie4jvmasfehljmad2fr10ri.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.SignInScheme = IdentityConstants.ExternalScheme;
})
.AddFacebook(options =>
{
    options.ClientId = "394521080004022";
    options.ClientSecret = "871471e68ad1ea6f28e85bc23fdebaf4";
})
;

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireRole("Admin");
        policy.RequireAssertion(context => context.User.IsInRole("Admin"));
    });
    options.AddPolicy("StaffPolicy", policy =>
    {
        policy.RequireRole("Staff");
        policy.RequireAssertion(context => context.User.IsInRole("Staff"));
    });
    options.AddPolicy("AdminAndStaffPolicy", policy =>
    {
        policy.RequireRole("Admin", "Staff");
        policy.RequireAssertion(context => context.User.IsInRole("Admin") || context.User.IsInRole("Staff"));
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


