using Microsoft.AspNetCore.Authentication.Cookies;
using Test3.Account.ClaimManager;
using Test3.Account.RoleManager;
using Test3.Account.UserManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("CartAccessPolicy", policy =>
        policy.RequireClaim("PERMISSIONS", "Cart"));

    options.AddPolicy("AdminPanelAccessPolicy", policy =>
        policy.RequireClaim("PERMISSIONS", "AdminPanel"));

    options.AddPolicy("UserDataAccessPolicy", policy =>
        policy.RequireClaim("PERMISSIONS", "EmployeesData"));

    options.AddPolicy("UserCartAccessPolicy", policy =>
        policy.RequireClaim("PERMISSIONS", "UserCart"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddSingleton<RoleModel>();
builder.Services.AddSingleton<IRoleAction, RoleAction>();
builder.Services.AddSingleton<UserModel>();
builder.Services.AddSingleton<IUserAction, UserAction>();

builder.Services.AddSingleton<IClaimAction, ClaimAction>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthentication(); 
app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();