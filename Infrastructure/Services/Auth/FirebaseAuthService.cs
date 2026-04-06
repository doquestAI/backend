using Domain.Interfaces.Services;
using FirebaseAdmin.Auth;

namespace Infrastructure.Auth;

internal sealed class FirebaseAuthService : IFirebaseAuthService
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
