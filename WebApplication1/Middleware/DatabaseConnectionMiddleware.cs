using DataAcessLayer;

namespace WebApplication1.Middleware;

public class DatabaseConnectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DatabaseConnectionService _dbService;

    public DatabaseConnectionMiddleware(RequestDelegate next, DatabaseConnectionService dbService)
    {
        _next = next;
        _dbService = dbService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check database connection status and store in HttpContext
        var isConnected = _dbService.IsDatabaseConnected();
        context.Items["DatabaseConnected"] = isConnected;

        await _next(context);
    }
}

public static class DatabaseConnectionMiddlewareExtensions
{
    public static IApplicationBuilder UseDatabaseConnectionCheck(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DatabaseConnectionMiddleware>();
    }
}

