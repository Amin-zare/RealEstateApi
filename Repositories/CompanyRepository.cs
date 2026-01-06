using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _db;

    public CompanyRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Company>> GetCompaniesAsync(int skip, int take, CancellationToken ct = default)
    {
        return _db.Companies
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
}
