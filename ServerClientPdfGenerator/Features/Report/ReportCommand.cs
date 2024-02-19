using MediatR;
using Azure.Storage.Blobs;
using Hangfire;
using Hangfire.Server;
using IronPdf.Rendering;
using Microsoft.AspNetCore.SignalR;
using ServerClientPdfGenerator.Hubs;
using ServerClientPdfGenerator.Services;
using ServerClientPdfGenerator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ServerClientPdfGenerator.UIServices;

namespace ServerClientPdfGenerator.Features.Report;

public class ReportCommand : IRequest<string>
{
    public string UserId { get; set; }
}

public class ReportDto
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string UserId { get; set; } 
}

public class ReportErrorDto
{
    public string UserId { get; set; }
    public string ErrorMessage { get; set; }
}

public class ReportCommandHandler(
    IBackgroundJobClient backgroundJobClient,
    ILogger<ReportCommandHandler> logger,
    IHubContext<ReportsHub, IReportsHub> hubContext,
    IConfiguration configuration,
    IViewRenderService viewRenderService,
    IMediator mediator) : IRequestHandler<ReportCommand, string>
{
    public async Task<string> Handle(ReportCommand request, CancellationToken cancellationToken)
    {
        var jobId = backgroundJobClient.Enqueue(() =>
            StartBackgrounJob(request.UserId, null, CancellationToken.None));
        logger.LogInformation($"Job Sent Id: {jobId}...");

        return jobId;
    }

    public async Task StartBackgrounJob(string userId, PerformContext context, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"Creating Report for user Id: {userId}...");
            IronPdf.License.LicenseKey = configuration.GetValue<string>("IronPdf");
            var renderer = new IronPdf.ChromePdfRenderer();
            var marginNumber = 5;
            renderer.RenderingOptions.MarginTop = marginNumber;
            renderer.RenderingOptions.MarginLeft = marginNumber;
            renderer.RenderingOptions.MarginRight = marginNumber;
            renderer.RenderingOptions.MarginBottom = marginNumber;
            renderer.RenderingOptions.Timeout = 120;
            renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

            //*******
            // retrieve data from DB Here

            var userInfo = new UserModel() { FullName = "User Full Name", StoreName = "Store Name" };
            
            //

            var viewAsString = await
                viewRenderService.RenderToStringAsync("ReportPdf/ReportTest",
                    new ReportModel() { UserDto = userInfo });

            var pdf = renderer.RenderHtmlAsPdf(viewAsString).Stream;
            var result = new FileStreamResult(pdf, "application/pdf").FileStream;
            var fn = $"Report_{userId}_{DateTime.Now:MMddyyyy_hhmmss}.pdf";
            var storageConnectionString = configuration.GetValue<string>("ConnectionStrings:AzureStorage");
            var client = new BlobContainerClient(storageConnectionString, $"{BlobLocationNamesConstants.REPORTS}");
            await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            using var ms = new MemoryStream();
            await result.CopyToAsync(ms, cancellationToken);
            ms.Seek(0, SeekOrigin.Begin);
            //var bytes = ms.ToArray();
            var dto = new ReportDto()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FileName = fn
            };
            await client.UploadBlobAsync(dto.Id, ms);

            await hubContext.Clients.All.ReportReady(dto);
        }
        catch (Exception ex)
        {
            var errorDto = new ReportErrorDto()
            {
                UserId = userId,
                ErrorMessage = ex.Message
            };
            await hubContext.Clients.All.ReportError(errorDto);
        }
    }
}