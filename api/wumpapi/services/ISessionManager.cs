using Microsoft.AspNetCore.Identity;
using wumpapi.neo4j;
using wumpapi.Services;
using wumpapi.structures;

public interface ISessionManager
{
    /// <summary>
    /// Check if a user's details are correct, and if they are receive a pair containing a session and the authed user. Sessions expire after a certain <see cref="SessionID.Duration"/>
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <param name="userRepository">User repository service</param>
    /// <param name="passwordHasher">Password hasher service</param>
    /// <returns>Tuple {User ID, User}</returns>
    /// <exception cref="IncorrectPasswordException">Password is wrong</exception>
    Task<Tuple<string,User>> AuthUser(string username, string password, IUserRepository userRepository, IPasswordHasher<User> passwordHasher);
    /// <summary>
    /// Gets an already authed user
    /// </summary>
    /// <param name="userId">Session id of the user</param>
    /// <returns>The user that is associated with the given session</returns>
    /// <exception cref="SessionExpiredException">If the session is expired</exception>
    /// <exception cref="InvalidSessionException">If the session is not known (already expired+never existed)</exception>
    User GetAuthedUser(string userId);
    /// <summary>
    /// Logs a user out
    /// </summary>
    /// <param name="userId">sessionid</param>
    /// <returns>true</returns>
    /// <exception cref="InvalidSessionException">The session didn't exist, if it's expired it logs them out anyway</exception>
    bool Logout(string userId);
    /// <summary>
    /// Checks if a session is valid
    /// </summary>
    /// <param name="userId">session</param>
    /// <returns>If the session is valid (expired, not found etc.)</returns>
    bool IsSessionValid(string userId);
}