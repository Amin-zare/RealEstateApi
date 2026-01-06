using RealEstateApi.Dtos;
using RealEstateApi.Repositories;


namespace RealEstateApi.Endpoints;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/webhook/apartment-updated",
            async (
                HttpRequest req,
                IApartmentRepository apartmentRepo,
                ILogger<Program> logger,
                IConfiguration cfg,
                ApartmentUpdate dto) =>
            {
                var expected = cfg["WebhookSecret"];
                if (string.IsNullOrEmpty(expected))
                {
                    logger.LogWarning("Webhook secret is not configured.");
                    return Results.Unauthorized();
                }

                if (dto is null)
                    return Results.BadRequest("Invalid request body.");

                if (!req.Headers.TryGetValue("X-Webhook-Secret", out var providedHeader)
                    || providedHeader.Count != 1
                    || string.IsNullOrEmpty(providedHeader[0]))
                {
                    logger.LogWarning("Missing or invalid X-Webhook-Secret header.");
                    return Results.Unauthorized();
                }

                var providedBytes = System.Text.Encoding.UTF8.GetBytes(providedHeader[0]);
                var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);

                if (providedBytes.Length != expectedBytes.Length ||
                    !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                        providedBytes, expectedBytes))
                {
                    logger.LogWarning("Invalid webhook secret provided.");
                    return Results.Unauthorized();
                }

                if (dto.ApartmentId <= 0)
                    return Results.BadRequest("Invalid ApartmentId.");

                var apt = await apartmentRepo.FindByIdAsync(dto.ApartmentId);
                if (apt is null)
                    return Results.NotFound();

                if (dto.IsRenovated is not null)
                    apt.IsRenovated = dto.IsRenovated.Value;

                await apartmentRepo.SaveChangesAsync();

                logger.LogInformation(
                    "Webhook applied: ApartmentId {ApartmentId}",
                    dto.ApartmentId);

                return Results.NoContent();
            })
            .WithName("WebhookApartmentUpdated")
            .WithOpenApi();


        return app;
    }
}