using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Dtos;
using RealEstateApi.Services;
using System.Security.Cryptography;
using System.Text;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebhookController : ControllerBase
{
    private readonly IApartmentService _service;
    private readonly IConfiguration _cfg;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IApartmentService service,
        IConfiguration cfg,
        ILogger<WebhookController> logger)
    {
        _service = service;
        _cfg = cfg;
        _logger = logger;
    }

    [HttpPost("apartment-updated")]
    public async Task<IActionResult> ApartmentUpdated([FromBody] ApartmentUpdate dto)
    {
        var expected = _cfg["WebhookSecret"];
        if (string.IsNullOrEmpty(expected))
            return Unauthorized();

        if (!Request.Headers.TryGetValue("X-Webhook-Secret", out var provided))
            return Unauthorized();

        var providedBytes = Encoding.UTF8.GetBytes(provided!);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        if (!CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
            return Unauthorized();

        var updated = await _service.ApplyApartmentUpdateAsync(
            dto.ApartmentId, dto.IsRenovated);

        if (!updated)
            return NotFound();

        return NoContent();
    }
}
