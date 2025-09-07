
namespace RealEstateApi.Dtos;

public sealed record ApartmentUpdate(
    int ApartmentId,
    bool? IsRenovated
);