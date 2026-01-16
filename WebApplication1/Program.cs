using Auth0.AspNetCore.Authentication;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.Services;
using DataAcessLayer;
using DataAcessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

Console.WriteLine("Base path: " + Directory.GetCurrentDirectory());

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// AUTH0 Authentication Configuration
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0:Domain configuration is missing");
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? throw new InvalidOperationException("Auth0:ClientId configuration is missing");
    options.Scope = "openid profile email";
});

// Razor Pages Configuration
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Register");
    options.Conventions.AllowAnonymousToPage("/Logout");
    options.Conventions.AllowAnonymousToPage("/Callback");
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

app.UseAuthentication();

// Middleware to add role claim from database after Auth0 authentication
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        try
        {
            // Get user repository from service provider
            var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
            
            // Get username from Auth0 claims (try multiple sources)
            var emailClaim = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var nameClaim = context.User.FindFirst(ClaimTypes.Name)?.Value ?? context.User.FindFirst("name")?.Value;
            var subClaim = context.User.FindFirst("sub")?.Value;
            
            var username = emailClaim ?? nameClaim ?? subClaim;
            
            if (!string.IsNullOrEmpty(username))
            {
                // Get user from database
                var user = userRepository.GetUserByUsername(username);
                
                if (user != null && !string.IsNullOrEmpty(user.role))
                {
                    // Check if database role is already in claims
                    var hasDbRole = context.User.HasClaim(ClaimTypes.Role, user.role);
                    
                    // Always add/update database role to ensure it's available
                    if (!hasDbRole)
                    {
                        // Create new identity copying all existing claims
                        var identity = new ClaimsIdentity(
                            context.User.Identity.AuthenticationType,
                            context.User.Identity.NameClaimType,
                            context.User.Identity.RoleClaimType);
                        
                        // Copy all existing claims
                        foreach (var claim in context.User.Claims)
                        {
                            identity.AddClaim(claim);
                        }
                        
                        // Add role claim from database
                        identity.AddClaim(new Claim(ClaimTypes.Role, user.role));
                        
                        // Replace the user principal
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't break the request
            Console.WriteLine($"Error adding role claim: {ex.Message}");
        }
    }
    
    await next();
});

app.UseAuthorization();

// Configure default route to redirect to Dashboard
app.MapGet("/", () => Results.Redirect("/Dashboard"));

app.MapRazorPages();

app.Run();
