using CreditApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreditApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditController : ControllerBase
{
    private readonly CreditService _service;

    public CreditController(CreditService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await _service.GetAsync(id));
}