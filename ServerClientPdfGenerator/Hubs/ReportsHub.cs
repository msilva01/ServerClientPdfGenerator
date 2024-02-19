using Microsoft.AspNetCore.SignalR;
using ServerClientPdfGenerator.Features.Report;

namespace ServerClientPdfGenerator.Hubs;

public interface IReportsHub
{
    [HubMethodName("reportReady")]
    Task ReportReady(ReportDto fileInfo);
    [HubMethodName("reportError")]
    Task ReportError(ReportErrorDto errorDto);
}

public class ReportsHub : Hub<IReportsHub>
{
    
}