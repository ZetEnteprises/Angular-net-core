using AngularApp1.Server.Controllers.models;
using AngularApp1.Server.services;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers
{
  [ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly CalculatorService _calculatorService;

    public CalculatorController(CalculatorService calculatorService)
    {
        _calculatorService = calculatorService;
    }

    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] CalculationRequest request)
    {
        var result = _calculatorService.CalculateResult(request.Operation);
        return Ok(new { result });
    }

    [HttpGet("operations")]
    public async Task<IActionResult> GetOperations()
    {
        var operations = await _calculatorService.GetOperationsAsync();
        return Ok(operations);
    }
}

}
