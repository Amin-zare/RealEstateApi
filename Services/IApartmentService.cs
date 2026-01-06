using RealEstateApi.Models;

namespace RealEstateApi.Services;

public interface IApartmentService
{
    Task<List<Apartment>> GetCompanyApartmentsAsync(int companyId, int skip, int take, CancellationToken ct);

    Task<List<Apartment>> GetExpiringApartmentsAsync(int companyId, DateTime limitUtc);

    Task<bool> ApplyApartmentUpdateAsync(int apartmentId, bool? isRenovated);
}
