using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.Abstracts;

internal abstract class Archive : Entity
{
    public bool IsActive { get; protected set; }
    public BigString? CloudStorageKey { get; protected set; }
    public ContentType? ContentType { get; protected set; }
    public DateTime? SignedUrlExpiration { get; protected set; }
    public Url? SignedUrl { get; protected set; }
    public string? ContainerName { get; protected set; }
    public BigString? LocalTemporaryPath { get; protected set; }

    protected Archive() { }

    public Archive(BigString? cloudStorageKey, BigString localTemporaryPath)
    {
        AddNotificationsFromValueObjects(cloudStorageKey, localTemporaryPath);
        CloudStorageKey = cloudStorageKey;
        LocalTemporaryPath = localTemporaryPath;
    }

    public Archive(BigString localTemporaryPath)
    {
        AddNotificationsFromValueObjects(localTemporaryPath);
        LocalTemporaryPath = localTemporaryPath;
    }


    public void SetTemporaryUrl(Url url, DateTime expiration)
    {
        AddNotificationsFromValueObjects(url);
        SignedUrl = url;
        SignedUrlExpiration = expiration;
    }

    public void SetCloudStorageKey(BigString? cloudStorageKey)
    {
        AddNotificationsFromValueObjects(cloudStorageKey);
        CloudStorageKey = cloudStorageKey;
    }
}
