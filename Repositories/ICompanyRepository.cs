using RealEstateApi.Models;

namespace RealEstateApi.Repositories;

public interface ICompanyRepository
{
    Task<List<Company>> GetCompaniesAsync(int skip, int take, CancellationToken ct = default);
}
