using Domain.Common.Responses;
using Flunt.Notifications;

namespace Application.UseCases.Queries.User.GetOne;

internal record AddressData(
    long? Number,
    string? Street,
    string? NeighBordHood,
    string? CEP,
    string? Complement
);

internal record Response(
    int StatusCode,
    string? Message = null,
    List<Notification>? Notifications = null,
    Guid Id = default,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    bool Active = false,
    bool IsActiveCredentials = false,
    AddressData? Address = null) : BaseResponse(StatusCode, Message, Notifications);
