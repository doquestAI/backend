using Flunt.Notifications;

namespace Application.Common;

public class Result<T>
{
    private Result(T? value, bool isSuccess, IEnumerable<Notification> notifications)
    {
        Value = value;
        IsSuccess = isSuccess;
        Notifications = notifications.ToList();
    }

    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Notification> Notifications { get; }

    public static Result<T> Success(T value) => new(value, true, []);
    public static Result<T> Failure(IEnumerable<Notification> notifications) => new(default, false, notifications);
    public static Result<T> Failure(string key, string message) =>
        new(default, false, [new Notification(key, message)]);
}

internal static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(IEnumerable<Notification> notifications) => Result<T>.Failure(notifications);
    public static Result<T> Failure<T>(string key, string message) => Result<T>.Failure(key, message);
}
