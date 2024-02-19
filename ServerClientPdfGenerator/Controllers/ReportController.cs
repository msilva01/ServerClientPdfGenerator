using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServerClientPdfGenerator.Features.Report;

namespace ServerClientPdfGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController (IMediator mediator, ILogger<ReportController> logger) : Controller
{
    
    [HttpPost("[action]")]
    public async Task<IActionResult> GenerateReport([FromBody] ReportCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug($"Generating Report for user {request.UserId}");

        var response = await mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}