using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.User.GetAll;

internal record UserData(
    Guid Id,
    string FirstName,
    string? LastName,
    string Email,
    bool Active,
    bool IsActiveCredentials
);

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    List<UserData>? Items = null,
    int TotalCount = 0,
    int PageNumber = 0,
    int PageSize = 0,
    int TotalPages = 0,
    bool HasPreviousPage = false,
    bool HasNextPage = false) : BaseResponse(StatusCode, Message, Notifications);
