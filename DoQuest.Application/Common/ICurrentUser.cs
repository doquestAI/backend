namespace DoQuest.Application.Common;

public interface ICurrentUser
{
    string FirebaseUid { get; }
    bool IsAuthenticated { get; }
}
