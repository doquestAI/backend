namespace Domain.Interfaces.Services.Cloud.Auth;

internal interface IAuthService
{
    Task<string?> GetEmailFromUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}