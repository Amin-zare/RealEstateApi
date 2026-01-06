using RealEstateApi.Models;
using RealEstateApi.Repositories;

namespace RealEstateApi.Services;

public sealed class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repo;

    public CompanyService(ICompanyRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Company>> GetCompaniesAsync(int skip, int take, CancellationToken ct)
        => _repo.GetCompaniesAsync(skip, take, ct);
}
