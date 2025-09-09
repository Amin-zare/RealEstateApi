namespace RealEstateApi.Extensions
{
    public static class CorsSetup
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var rawOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            var allowedOrigins = rawOrigins
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim().TrimEnd('/'))
                .Where(o => !string.IsNullOrEmpty(o))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var allowCredentials = configuration.GetValue<bool>("Cors:AllowCredentials", false);

            if (allowedOrigins.Length == 0)
                throw new InvalidOperationException("Please configure 'Cors:AllowedOrigins' in appsettings.");

            if (allowCredentials && (allowedOrigins.Length == 1 && allowedOrigins[0] == "*"))
                throw new InvalidOperationException("Cannot use AllowCredentials with wildcard origins. Specify explicit origins in 'Cors:AllowedOrigins'.");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .WithHeaders("Authorization", "Accept", "Content-Type", "X-Webhook-Secret")
                          .WithMethods("GET", "POST");
                    if (allowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                });
            });

            return services;
        }
    }
}