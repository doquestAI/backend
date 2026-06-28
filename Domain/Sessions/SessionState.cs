namespace Domain.Sessions;

public enum SessionState
{
    Active = 0,
    Paused = 1,
    Closed = 2,
    Expired = 3,
}
