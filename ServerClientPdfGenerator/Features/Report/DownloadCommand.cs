using MediatR;
using ServerClientPdfGenerator.UIServices;

namespace ServerClientPdfGenerator.Features.Report;

public class DownloadCommand : IRequest<byte[]>
{
    public string Filename { get; set; }
    public string Name { get; set; }
}

public class DownloadHandler(IStorageProvider storageProvider, ILogger<DownloadHandler> logger)
    : IRequestHandler<DownloadCommand, byte[]>
{
    public async Task<byte[]> Handle(DownloadCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug($"Download for file Name {request.Filename} - Name of the file {request.Name}");
        var result = await storageProvider.DownloadAsync(request.Filename, BlobLocationNamesConstants.REPORTS,
            cancellationToken);

        return result;
    }
}