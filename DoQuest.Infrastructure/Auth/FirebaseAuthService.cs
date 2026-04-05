using DoQuest.Application.Abstractions;
using FirebaseAdmin.Auth;

namespace DoQuest.Infrastructure.Auth;

public sealed class FirebaseAuthService : IFirebaseAuthService
{
    public async Task<string?> GetEmailFromUidAsync(string firebaseUid, CancellationToken cancellationToken = default)
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
