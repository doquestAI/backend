using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.Document.GetById;

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid DocumentId = default,
    string? FileName = null,
    long? FileSizeBytes = null,
    string? Status = null,
    int? ChunksGenerated = null) : BaseResponse(StatusCode, Message, Notifications);
