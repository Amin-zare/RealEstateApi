using System.ComponentModel.DataAnnotations;
namespace RealEstateApi.Models;

public class Apartment
{
    public int Id { get; set; }
    [MaxLength(200)]
    public string Address { get; set; } = default!;
    public DateTime? LeaseEnd { get; set; }
    public bool IsRenovated { get; set; }
    public int CompanyId { get; set; }
    // Navigation property: each apartment belongs to one company
    public Company? Company { get; set; }
}