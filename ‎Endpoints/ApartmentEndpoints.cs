using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;

namespace RealEstateApi.Endpoints;

public static class ApartmentEndpoints
{
    public static IEndpointRouteBuilder MapApartmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/companies/{companyId:int}/apartments",
            async (int companyId, AppDbContext db, ILogger<Program> logger) =>
            {
                var apartments = await db.Apartments
                    .Where(a => a.CompanyId == companyId)
                    .AsNoTracking()
                    .ToListAsync();

                logger.LogInformation("Fetched {Count} apartments for CompanyId {CompanyId}",
                    apartments.Count, companyId);

                return Results.Ok(apartments);
            })
            .WithName("GetCompanyApartments")
            .WithOpenApi();

        app.MapGet("/companies/{companyId:int}/apartments/expiring",
            async (int companyId, AppDbContext db, ILogger<Program> logger) =>
            {
                var limit = DateTime.UtcNow.AddMonths(3);

                var apartments = await db.Apartments
                    .Where(a => a.CompanyId == companyId && a.LeaseEnd != null && a.LeaseEnd <= limit)
                    .AsNoTracking()
                    .ToListAsync();

                logger.LogInformation("Fetched {Count} expiring apartments (LeaseEnd <= {Limit}) for CompanyId {CompanyId}",
                    apartments.Count, limit, companyId);

                return Results.Ok(apartments);
            })
            .WithName("GetExpiringApartments")
            .WithOpenApi();

        return app;
    }
}