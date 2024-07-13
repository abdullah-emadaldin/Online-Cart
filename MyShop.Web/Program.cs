using Microsoft.EntityFrameworkCore;
using MyShop.Model;
using MyShop.EFCore;
using MyShop.EFCore.Data;
using MyShop.EFCore.Repositories;
using MyShop.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyShop.Model.Models;
using Stripe;
using MyShop.EFCore.Initialize;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("cs")).UseLazyLoadingProxies());
builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

//builder.Services.AddIdentity<User,IdentityRole>(o=>o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1)).AddDefaultTokenProviders().AddDefaultUI().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(o =>  o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1)).AddDefaultTokenProviders().AddDefaultUI().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddSingleton<IEmailSender,EmailSender>();
//builder.Services.AddScoped<ICategory, CategoryRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:secretkey").Get<string>();

SeedDb();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Customer",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDb()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
