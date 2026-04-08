using Domain.Interfaces.Services.Cloud.Auth;
using FirebaseAdmin.Auth;

namespace Infrastructure.Services.GoogleCloud.Auth;

internal sealed class FirebaseAuthService : IAuthService
{
    public async Task<string?> GetEmailFromUidAsync(string firebaseUid,
         CancellationToken cancellationToken = default)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseUid, cancellationToken);
            return userRecord.Email;
        }
        catch (FirebaseAuthException)
        {
            return null;
        }
    }
}