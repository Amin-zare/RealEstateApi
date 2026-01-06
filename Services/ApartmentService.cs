using RealEstateApi.Models;
using RealEstateApi.Repositories;

namespace RealEstateApi.Services;

public sealed class ApartmentService : IApartmentService
{
    private readonly IApartmentRepository _repo;

    public ApartmentService(IApartmentRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Apartment>> GetCompanyApartmentsAsync(
        int companyId,
        int skip,
        int take,
        CancellationToken ct)
    {
        return _repo.GetCompanyApartmentsAsync(companyId, skip, take, ct);
    }

    public Task<List<Apartment>> GetExpiringApartmentsAsync(int companyId, DateTime limitUtc)
    {
        return _repo.GetExpiringApartmentsAsync(companyId, limitUtc);
    }

    public async Task<bool> ApplyApartmentUpdateAsync(int apartmentId, bool? isRenovated)
    {
        var apt = await _repo.FindByIdAsync(apartmentId);
        if (apt is null)
            return false;

        if (isRenovated is not null)
            apt.IsRenovated = isRenovated.Value;

        await _repo.SaveChangesAsync();
        return true;
    }
}
