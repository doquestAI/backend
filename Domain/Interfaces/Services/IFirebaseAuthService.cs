namespace Domain.Interfaces.Services;

internal interface IFirebaseAuthService
{
    Task<string?> GetEmailFromUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}
