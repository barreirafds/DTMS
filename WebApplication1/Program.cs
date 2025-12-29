using Auth0.AspNetCore.Authentication;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Services;
using DataAcessLayer;
using DataAcessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WebApplication1.Middleware;

Console.WriteLine("Base path: " + Directory.GetCurrentDirectory());

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=mssqlstud.fhict.local;Database=dbi570286_dbdtms1;User Id=dbi570286_dbdtms1;Password=root1234;TrustServerCertificate=True;";

// Register Database Connection Service as Singleton to check connection status
builder.Services.AddSingleton<DatabaseConnectionService>(provider => 
    new DatabaseConnectionService(connectionString));

// AUTH0 Authentication Configuration
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0:Domain configuration is missing");
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? throw new InvalidOperationException("Auth0:ClientId configuration is missing");
    options.Scope = "openid profile email";
});

;

// Razor Pages Configuration
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Register");
    options.Conventions.AllowAnonymousToPage("/Logout");
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddControllersWithViews();

// Repository Registrations (Data Access Layer)
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Service Registrations (Business Logic Layer)
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Check database connection on startup
var dbService = app.Services.GetRequiredService<DatabaseConnectionService>();
var isConnected = dbService.IsDatabaseConnected();

if (!isConnected)
{
    // Log warning if database is not connected
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning("Database is not connected. The application will run in limited mode.");
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable request body buffering for reading body multiple times
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Use database connection check middleware
app.UseDatabaseConnectionCheck();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
