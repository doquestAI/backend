namespace Application.Common;

internal interface ICurrentUser
{
    string FirebaseUid { get; }
    bool IsAuthenticated { get; }
}