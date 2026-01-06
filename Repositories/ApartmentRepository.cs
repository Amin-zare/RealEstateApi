using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories;

public sealed class ApartmentRepository : IApartmentRepository
{
    private readonly AppDbContext _db;

    public ApartmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Apartment>> GetCompanyApartmentsAsync(int companyId, int skip, int take, CancellationToken ct = default)
    {
        return _db.Apartments
            .Where(a => a.CompanyId == companyId)
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public Task<List<Apartment>> GetExpiringApartmentsAsync(int companyId, DateTime limitUtc)
    {
        return _db.Apartments
            .Where(a => a.CompanyId == companyId && a.LeaseEnd != null && a.LeaseEnd <= limitUtc)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Apartment?> FindByIdAsync(int apartmentId, CancellationToken ct = default)
    {
        return _db.Apartments.FindAsync([apartmentId], ct).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
