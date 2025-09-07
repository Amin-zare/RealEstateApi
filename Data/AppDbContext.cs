using Microsoft.EntityFrameworkCore;
using RealEstateApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RealEstateApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options)
    {
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Apartment> Apartments => Set<Apartment>();
    }
}