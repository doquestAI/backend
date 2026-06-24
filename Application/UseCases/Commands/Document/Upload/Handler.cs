using Domain.Configurations;
using Domain.ExtensionsMethods;
using Domain.Interfaces.Context;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Domain.ValueObjects;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;
using Microsoft.Extensions.Options;
using DocumentEntity = Domain.Entities.Core.Document;

namespace Application.UseCases.Commands.Document.Upload;

internal class Handler(
    IDocumentRepository documentRepository,
    IDbCommit dbCommit,
    ITemporaryStorageService temporaryStorageService,
    IMessagePublisher messagePublisher,
    IUserContext userContext,
    IOptions<AzureSettings> AzureSettings,
    IOptions<ServiceBusSettings> pubSubSettings,
    IOptions<AppSettings> appSettings) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notifiable<Notification>>()
            .Requires()
            .IsNotNull(request.File, "File", "File is required")
            .IsGreaterThan(request.File?.Length ?? 0, 0, "File", "File cannot be empty");

        if (!contract.IsValid)
            return new Response(
                StatusCode: 400,
                Message: "Request invalid",
                Notifications: contract.Notifications.ToList());

        var contentType = ContentTypeExtensions.ParseMimeType(request.File!.ContentType);
        if (contentType == null)
            return new Response(
                StatusCode: 400,
                Message: "Unsupported file type");

        var fileName = new BigString(request.File.FileName);
        var extension = Path.GetExtension(request.File.FileName);

        var document = new DocumentEntity(
            fileName: fileName,
            containerName: AzureSettings.Value.ContainerDocuments,
            contentType: contentType.Value,
            fileSizeBytes: request.File.Length,
            uploadedByUserId: userContext.UserId,
            localTemporaryPath: new BigString(""));

        await using var stream = request.File.OpenReadStream();
        var localPath = await temporaryStorageService.SaveAsync(
            appSettings.Value.TempFilesPath,
            document.Id,
            stream,
            extension,
            cancellationToken);

        document.UpdateLocalPath(new BigString(localPath));

        await documentRepository.CreateAsync(document, cancellationToken);
        await dbCommit.Commit(cancellationToken);

        var objectPath = $"documents/{userContext.UserId}/{document.Id}/{request.File.FileName}";

        var uploadMessage = new StorageUploadMessage(
            DocumentId: document.Id,
            ContainerName: AzureSettings.Value.ContainerDocuments,
            ObjectPath: objectPath,
            ContentType: request.File.ContentType,
            LocalFilePath: localPath);

        await messagePublisher.PublishAsync(
            pubSubSettings.Value.Queues.FileUpload,
            uploadMessage,
            cancellationToken);

        return new Response(
            StatusCode: 202,
            Message: "Document upload initiated",
            Notifications: null,
            DocumentId: document.Id,
            FileName: document?.FileName?.Body?.ToString(),
            FileSizeBytes: document?.FileSizeBytes,
            Status: document?.Status.ToString()
        );
    }
}