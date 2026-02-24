using CreditApp.Api.Services;
using CreditApp.Domain.Data;
using Microsoft.AspNetCore.Mvc;

namespace CreditApp.Api.Controllers;

/// <summary>
/// Контроллер для работы с кредитными заявками
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CreditController : ControllerBase
{
    private readonly ICreditService _creditService;
    private readonly ILogger<CreditController> _logger;

    public CreditController(
        ICreditService creditService,
        ILogger<CreditController> logger)
    {
        _creditService = creditService;
        _logger = logger;
    }

    /// <summary>
    /// Получить кредитную заявку по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CreditApplication), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreditApplication>> GetCreditApplication(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting credit application with Id: {CreditId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid credit application Id: {CreditId}", id);
            return BadRequest("Id must be positive number");
        }

        try
        {
            var creditApplication = await _creditService.GetAsync(id, cancellationToken);

            _logger.LogInformation(
                "Successfully retrieved credit application {CreditId}",
                id);

            return Ok(creditApplication);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credit application {CreditId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Сгенерировать кредитную заявку с указанным seed
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CreditApplication), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreditApplication>> GenerateCredit(
        [FromQuery] int? seed,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating credit application with seed: {Seed}", seed);

        var id = new Random().Next(1, 10000);

        CreditApplication creditApplication;

        if (seed.HasValue)
        {
            creditApplication = await _creditService.GetAsync(id, seed.Value, cancellationToken);
            _logger.LogInformation("Generated credit application with seed {Seed}: {CreditId}", seed, id);
        }
        else
        {
            creditApplication = await _creditService.GetAsync(id, cancellationToken);
            _logger.LogInformation("Generated random credit application: {CreditId}", id);
        }

        return Ok(creditApplication);
    }

    /// <summary>
    /// Удалить заявку из кэша
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveCreditApplication(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive number");
        }

        await _creditService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}