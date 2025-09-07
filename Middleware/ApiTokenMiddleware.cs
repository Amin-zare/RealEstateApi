namespace RealEstateApi.Middleware;

public class ApiTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _expected;

    public ApiTokenMiddleware(RequestDelegate next, IConfiguration cfg)
    {
        _next = next;
        var apiToken = cfg["ApiToken"];
        if (string.IsNullOrWhiteSpace(apiToken))
            throw new InvalidOperationException("Missing ApiToken configuration. Please set 'ApiToken' in your appsettings or environment variables.");
        _expected = apiToken;
    }

    public async Task InvokeAsync(HttpContext context)
    {
    var auth = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(auth) ||
            !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing token");
            return;
        }

        var provided = auth.Substring("Bearer ".Length).Trim();

        if (!string.Equals(provided, _expected, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next(context);
    }
}