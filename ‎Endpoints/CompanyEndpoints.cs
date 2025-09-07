using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;

namespace RealEstateApi.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/companies",
            async (AppDbContext db, ILogger<Program> logger) =>
            {
                var companies = await db.Companies
                    .AsNoTracking()
                    .ToListAsync();

                logger.LogInformation("Fetched {Count} companies from database", companies.Count);

                return Results.Ok(companies);
            })
            .WithName("GetCompanies")
            .WithOpenApi();

        return app;
    }
}