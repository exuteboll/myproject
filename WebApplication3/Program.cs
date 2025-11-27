using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DAL;
using WebApplication3.DAL.Interfaces;
using WebApplication3.DAL.Storage;
using WebApplication3.Services.Interfaces;
using WebApplication3.Services.Realizations;
using WebApplicatoin3.Domain.ModelsDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/Privacy";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
    });

// Register services
builder.Services.AddScoped<IBaseStorage<UserDb>, UserStorage>();
builder.Services.AddScoped<IBaseStorage<ProductDb>, ProductStorage>();
builder.Services.AddScoped<IBaseStorage<CategoryDb>, CategoryStorage>();
builder.Services.AddScoped<IBaseStorage<OrderDb>, OrderStorage>();
builder.Services.AddScoped<IBaseStorage<CartItemDb>, CartStorage>();
builder.Services.AddScoped<IBaseStorage<RequestDb>, BaseStorage<RequestDb>>();
builder.Services.AddScoped<IBaseStorage<ProductImageDb>, BaseStorage<ProductImageDb>>();
builder.Services.AddScoped<IBaseStorage<OrderItemDb>, BaseStorage<OrderItemDb>>();

// Register specific storage interfaces
builder.Services.AddScoped<ICartStorage, CartStorage>();
builder.Services.AddScoped<IProductStorage, ProductStorage>();
builder.Services.AddScoped<ICategoryStorage, CategoryStorage>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminService, AdminService>(); // ← ДОБАВЬТЕ ЭТУ СТРОКУ


// Other storages (если они используются напрямую)
builder.Services.AddScoped<UserStorage>();
builder.Services.AddScoped<ProductStorage>();
builder.Services.AddScoped<CategoryStorage>();
builder.Services.AddScoped<OrderStorage>();
builder.Services.AddScoped<ProductImageStorage>();
builder.Services.AddScoped<CartStorage>();


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
if (app.Environment.IsDevelopment())
{
    app.MapControllerRoute(
        name: "seed",
        pattern: "seed",
        defaults: new { controller = "Seed", action = "Index" });
}

app.Run();
