using wumpapi.Services;
using wumpapi.structures;

public interface ISessionManager
{
    Task<Tuple<string,User>> AuthUser(string username, string password);
    User GetAuthedUser(string userId);
    bool Logout(string userId);
    bool IsSessionValid(string userId);
}