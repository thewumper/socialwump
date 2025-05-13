using wumpapi.Services;
using wumpapi.structures;

public interface ISessionManager
{
    Task<Tuple<SessionID,User>> AuthUser(string username, string password);
    Task<User> GetAuthedUser(SessionID userId);
    Task<bool> Logout(SessionID userId);
}