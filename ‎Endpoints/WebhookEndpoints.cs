using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Dtos;

namespace RealEstateApi.Endpoints;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/webhook/apartment-updated",
            async (HttpRequest req,
            AppDbContext db,
                   ILogger<Program> logger,
                   IConfiguration cfg,
                   ApartmentUpdate dto) =>
            {
                // secret
                var expected = cfg["WebhookSecret"];
                if (string.IsNullOrEmpty(expected))
                {
                    logger.LogError("Missing WebhookSecret in configuration. Rejecting webhook.");
                    return Results.Unauthorized();
                }
                var provided = req.Headers["X-Webhook-Secret"].ToString();
                if (string.IsNullOrEmpty(provided) || provided != expected)
                {
                    logger.LogWarning("Invalid WebhookSecret provided. Rejecting webhook.");
                    return Results.Unauthorized();
                }

                // Basic validation
                if (dto.ApartmentId <= 0) return Results.BadRequest("Invalid ApartmentId.");

                // Apply update
                var apt = await db.Apartments.FirstOrDefaultAsync(a => a.Id == dto.ApartmentId);
                if (apt is null)
                {
                    logger.LogWarning("Apartment not found for webhook update. Id: {ApartmentId}", dto.ApartmentId);
                    return Results.NoContent();
                }

                if (dto.IsRenovated is not null) apt.IsRenovated = dto.IsRenovated.Value;

                await db.SaveChangesAsync();

                logger.LogInformation("Webhook applied: ApartmentId {ApartmentId}",
                     dto.ApartmentId);

                return Results.NoContent();
            })
            .WithName("WebhookApartmentUpdated")
            .WithOpenApi();

        return app;
    }
}
