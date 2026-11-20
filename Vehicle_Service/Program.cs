using Azure.Messaging.ServiceBus;
using VehicleService.Messaging;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// MVC services
builder.Services.AddControllersWithViews();

// Session configuration
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cookie authentication for MVC pages
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Customer_Login/Login_Pgload";
        options.AccessDeniedPath = "/Customer_Login/Login_Pgload";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = false;
    });

builder.Services.AddAuthorization();

// HTTP client for calling VehicleServiceAPI
builder.Services.AddHttpClient();

// Azure Service Bus (optional)
if (builder.Configuration.GetValue<bool>("Messaging:Enabled"))
{
    builder.Services.AddSingleton(new ServiceBusClient(
        builder.Configuration["ServiceBus:ConnectionString"]));
    builder.Services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();
}
else
{
    builder.Services.AddSingleton<IMessagePublisher, NoOpMessagePublisher>();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer_Login}/{action=Login_pgload}/{id?}");

app.Run();
