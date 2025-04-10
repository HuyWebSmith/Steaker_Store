using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Steaker_Store.Models;
using Steaker_Store.Models.Momo;
using Steaker_Store.Models.Vnpay;
using Steaker_Store.Repositories;
using Steaker_Store.Services.Momo;
using Steaker_Store.Services.VnPay;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
// Api MoMo
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();


// sql
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Đặt trước AddControllersWithViews(); 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { 
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.LogoutPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<IMenuItemRepository, EFMeunuItemRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
//// Api VNPAY
//builder.Services.Configure<VnpaySettings>(builder.Configuration.GetSection("Vnpay"));


//OAuth Google
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(facebookOptions => {
         // Đọc cấu hình
         IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
         facebookOptions.AppId = facebookAuthNSection["AppId"];
         facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
         // Thiết lập đường dẫn Facebook chuyển hướng đến
         facebookOptions.CallbackPath = "/signin-facebook";
     });

//Api VnPay
builder.Services.AddScoped<IVnPayService, VnPayService>();


// Chat Hub
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// Đặt trước UseRouting 
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.MapRazorPages();

app.MapStaticAssets();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<ChatHub>("/chathub");
app.Run();
