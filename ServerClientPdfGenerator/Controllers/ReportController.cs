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
    
    [HttpGet("[action]")]
    public async Task<FileResult> Download(string fileName, string name, CancellationToken cancellationToken)
    {
        logger.LogDebug($"Download Case History Report : {fileName} - name of the file: {name} ");
        var response = await mediator.Send(new DownloadCommand() { Filename=fileName, Name=name }, cancellationToken);

        return File(response, "application/octet-stream", name);
    }
}