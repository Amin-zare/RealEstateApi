using RealEstateApi.Repositories;

namespace RealEstateApi.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/companies",
            static async (
                ICompanyRepository companyRepo,
                ILogger<Program> logger,
                int? skip,
                int? take,
                CancellationToken ct) =>
            {
                const int DefaultTake = 50;
                const int MaxTake = 200;

                int actualSkip = (skip.HasValue && skip.Value >= 0) ? skip.Value : 0;
                int actualTake = (take.HasValue && take.Value > 0)
                    ? Math.Min(take.Value, MaxTake)
                    : DefaultTake;

                var companies = await companyRepo.GetCompaniesAsync(
                    actualSkip,
                    actualTake,
                    ct);

                logger.LogInformation(
                    "Fetched {Count} companies from database (skip={Skip}, take={Take})",
                    companies.Count, actualSkip, actualTake);

                return Results.Ok(companies);
            })
            .WithName("GetCompanies")
            .WithOpenApi();

        return app;
    }
}