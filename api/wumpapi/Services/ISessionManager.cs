using Microsoft.AspNetCore.Identity;
using wumpapi.neo4j;
using wumpapi.Services;
using wumpapi.structures;

public interface ISessionManager
{
    Task<Tuple<string,User>> AuthUser(string username, string password, IUserRepository userRepository, IPasswordHasher<User> passwordHasher);
    User GetAuthedUser(string userId);
    bool Logout(string userId);
    bool IsSessionValid(string userId);
}