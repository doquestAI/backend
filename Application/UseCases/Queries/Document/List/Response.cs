using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.Document.List;

internal record DocumentSummary(Guid Id, string? FileName, long FileSizeBytes, string Status, int? ChunksGenerated);

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    List<DocumentSummary>? Documents = null,
    int TotalCount = 0) : BaseResponse(StatusCode, Message, Notifications);
