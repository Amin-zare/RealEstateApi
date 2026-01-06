using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Services;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/apartments")]
public class ApartmentsController : ControllerBase
{
    private readonly IApartmentService _service;
    private readonly ILogger<ApartmentsController> _logger;

    public ApartmentsController(
        IApartmentService service,
        ILogger<ApartmentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanyApartments(
        int companyId,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        CancellationToken ct)
    {
        const int DefaultTake = 50;
        const int MaxTake = 200;

        int actualSkip = (skip.HasValue && skip.Value >= 0) ? skip.Value : 0;
        int actualTake = (take.HasValue && take.Value > 0) ? Math.Min(take.Value, MaxTake) : DefaultTake;

        var apartments = await _service.GetCompanyApartmentsAsync(
            companyId, actualSkip, actualTake, ct);

        return Ok(apartments);
    }

    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringApartments(int companyId)
    {
        var limit = DateTime.UtcNow.AddMonths(3);
        var apartments = await _service.GetExpiringApartmentsAsync(companyId, limit);
        return Ok(apartments);
    }
}
