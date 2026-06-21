using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Commands.Document.Upload;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid DocumentId = default,
    string? FileName = null,
    long? FileSizeBytes = null,
    string? Status = null) : BaseResponse(StatusCode, Message, Notifications);