using RealEstateApi.Models;

namespace RealEstateApi.Services;

public interface ICompanyService
{
    Task<List<Company>> GetCompaniesAsync(int skip, int take, CancellationToken ct);
}
