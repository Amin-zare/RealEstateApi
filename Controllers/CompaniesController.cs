using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Services;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(
        ICompanyService service,
        ILogger<CompaniesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        CancellationToken ct)
    {
        const int DefaultTake = 50;
        const int MaxTake = 200;

        int actualSkip = (skip.HasValue && skip.Value >= 0) ? skip.Value : 0;
        int actualTake = (take.HasValue && take.Value > 0) ? Math.Min(take.Value, MaxTake) : DefaultTake;

        var companies = await _service.GetCompaniesAsync(actualSkip, actualTake, ct);

        return Ok(companies);
    }
}
