namespace DoQuest.Application.Abstractions;

public interface IFirebaseAuthService
{
    Task<string?> GetEmailFromUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}
