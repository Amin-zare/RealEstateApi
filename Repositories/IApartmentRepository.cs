using RealEstateApi.Models;

namespace RealEstateApi.Repositories;

public interface IApartmentRepository
{
    Task<List<Apartment>> GetCompanyApartmentsAsync(int companyId, int skip, int take, CancellationToken ct = default);

    Task<List<Apartment>> GetExpiringApartmentsAsync(int companyId, DateTime limitUtc);

    Task<Apartment?> FindByIdAsync(int apartmentId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
