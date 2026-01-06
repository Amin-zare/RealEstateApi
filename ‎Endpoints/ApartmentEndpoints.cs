using RealEstateApi.Repositories;


namespace RealEstateApi.Endpoints;

public static class ApartmentEndpoints
{
    public static IEndpointRouteBuilder MapApartmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/companies/{companyId:int}/apartments",
            async (int companyId, IApartmentRepository apartmentRepo, ILogger<Program> logger, CancellationToken ct, int? skip, int? take) =>
            {
                const int DefaultTake = 50;
                const int MaxTake = 200;

                int actualSkip = (skip.HasValue && skip.Value >= 0) ? skip.Value : 0;
                int actualTake = (take.HasValue && take.Value > 0) ? Math.Min(take.Value, MaxTake) : DefaultTake;

                var apartments = await apartmentRepo.GetCompanyApartmentsAsync(
                    companyId: companyId,
                    skip: actualSkip,
                    take: actualTake,
                    ct: ct);

                logger.LogInformation(
                    "Fetched {Count} apartments for CompanyId {CompanyId} (skip={Skip}, take={Take})",
                    apartments.Count, companyId, actualSkip, actualTake);

                return Results.Ok(apartments);
            })
            .WithName("GetCompanyApartments")
            .WithOpenApi();

        app.MapGet("/companies/{companyId:int}/apartments/expiring",
            async (int companyId, IApartmentRepository apartmentRepo, ILogger<Program> logger) =>
            {
                var limit = DateTime.UtcNow.AddMonths(3);

                var apartments = await apartmentRepo.GetExpiringApartmentsAsync(
                    companyId: companyId,
                    limitUtc: limit);

                logger.LogInformation(
                    "Fetched {Count} expiring apartments (LeaseEnd <= {Limit}) for CompanyId {CompanyId}",
                    apartments.Count, limit, companyId);

                return Results.Ok(apartments);
            })
            .WithName("GetExpiringApartments")
            .WithOpenApi();


                return app;
            }
}