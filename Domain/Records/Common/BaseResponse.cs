using Flunt.Notifications;
using System.Text.Json.Serialization;

namespace Domain.Records;

internal record BaseResponse
{
    public int StatusCode { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Notification>? Notifications { get; init; }

    public BaseResponse(int statusCode, string? message = null, List<Notification>? notifications = null)
    {
        StatusCode = statusCode;
        Message = message;
        Notifications = notifications;
    }
}