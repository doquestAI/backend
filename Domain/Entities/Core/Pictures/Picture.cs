using Domain.Entities.Abstracts;
using Domain.Enums;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

internal class Picture : Archive
{
    [NotMapped]
    public AppFile? File { get; private set; }

    private Picture() { }
    public Picture(BigString? cloudStorageKey, bool isActive = false, AppFile appFile = null,
     BigString? localTemporaryPath = null, ContentType? contentType = null, string containerName = "")
    {
        AddNotificationsFromValueObjects(appFile);
        File = appFile;
        IsActive = isActive;
        CloudStorageKey = cloudStorageKey;
        LocalTemporaryPath = localTemporaryPath;
        ContainerName = containerName;
        ContentType = contentType;
    }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}