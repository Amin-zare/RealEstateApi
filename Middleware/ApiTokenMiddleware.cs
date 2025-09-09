using System.Security.Cryptography;
using System.Text;

namespace RealEstateApi.Middleware;

public class ApiTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _apiToken;
    private readonly string? _webhookSecret;

    public ApiTokenMiddleware(RequestDelegate next, IConfiguration cfg)
    {
        _next = next;
        _apiToken = cfg["ApiToken"];           
        _webhookSecret = cfg["WebhookSecret"]; 
        if (string.IsNullOrWhiteSpace(_apiToken) && string.IsNullOrWhiteSpace(_webhookSecret))
            throw new InvalidOperationException("At least one of ApiToken or WebhookSecret must be set.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string bearer = "Bearer ";
        var authHeaderValue = context.Request.Headers["Authorization"].ToString();
        var xSecret = context.Request.Headers["X-Webhook-Secret"].ToString().Trim();

        var hasAuth = !string.IsNullOrWhiteSpace(authHeaderValue);
        var hasX = !string.IsNullOrWhiteSpace(xSecret);

        bool ok = false;

        if (hasAuth)
        {
            if (authHeaderValue.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
            {
                var provided = authHeaderValue.Substring(bearer.Length).Trim();
                if (!string.IsNullOrEmpty(_apiToken))
                    ok |= TimeConstantEquals(provided, _apiToken);
            }
        }

        if (hasX && !string.IsNullOrEmpty(_webhookSecret))
        {
            ok |= TimeConstantEquals(xSecret, _webhookSecret);
        }

        if (ok)
        {
            string method = hasAuth ? "Bearer token" : hasX ? "Webhook secret" : "Unknown";
        }

        if (!ok)
        {
            string method = hasAuth ? "Bearer token" : hasX ? "Webhook secret" : "None";
            var statusCode = (hasAuth || hasX)
                ? StatusCodes.Status403Forbidden
                : StatusCodes.Status401Unauthorized;
            context.Response.StatusCode = statusCode;

            context.Response.Headers["Cache-Control"] = "no-store";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
            context.Response.ContentType = "text/plain; charset=utf-8";

            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.Headers["WWW-Authenticate"] = "Bearer realm=\"api\"";
            }

            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }

    private static bool TimeConstantEquals(string a, string b)
    {
        var ba = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(ba, bb);
    }
}