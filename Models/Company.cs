namespace RealEstateApi.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    // Navigation property: one company can have many apartments
    public List<Apartment> Apartments { get; set; } = new();
}