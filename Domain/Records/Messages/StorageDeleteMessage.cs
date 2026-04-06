namespace Domain.Messages;

internal record StorageDeleteMessage(
    string? ContainerName,
    string? ObjectPath);