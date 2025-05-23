using Microsoft.AspNetCore.Identity;
using Neo4j.Driver;
using wumpapi.neo4j;
using wumpapi.structures;

namespace wumpapi.Services;
/// <summary>
/// Service that manages authentication and sessions of active users
/// </summary>
public class SessionManager : ISessionManager
{
    private readonly ILogger<SessionManager> logger;

    private Dictionary<SessionID, User> users;
    private Dictionary<string, SessionID> sessions;
    public SessionManager(ILogger<SessionManager> logger)
    {
        this.logger = logger;
        users = new Dictionary<SessionID, User>();
        sessions = new Dictionary<string, SessionID>();
    }
    /// <summary>
    /// Check if a user's details are correct, and if they are receive a pair containing a session and the authed user. Sessions expire after a certain <see cref="SessionID.Duration"/>
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <param name="userRepository">User repository service</param>
    /// <param name="passwordHasher">Password hasher service</param>
    /// <returns>Tuple {User ID, User}</returns>
    /// <exception cref="IncorrectPasswordException">Password is wrong</exception>
    public async Task<Tuple<string,User>> AuthUser(String username, String password, IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        User user = await userRepository.GetUser(username,includePassword:true);
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Success)
        {
            user.Censor();
            SessionID sessionId = new SessionID();
            
            sessions.Add(sessionId.SSID, sessionId);
            users.Add(sessionId, user);
            return new Tuple<string,User>(sessionId.SSID, user);
        }
        else
        {
            throw new IncorrectPasswordException();
        }
    }
    /// <summary>
    /// Gets an already authed user
    /// </summary>
    /// <param name="userId">Session id of the user</param>
    /// <returns>The user that is associated with the given session</returns>
    /// <exception cref="SessionExpiredException">If the session is expired</exception>
    /// <exception cref="InvalidSessionException">If the session is not known (already expired+never existed)</exception>
    public User GetAuthedUser(string userId)
    {
        if (sessions.ContainsKey(userId))
        {
            SessionID sessionId = sessions[userId];
            if (sessionId.Expired)
            {
                Logout(userId);
                throw new SessionExpiredException();
            }
            User user = users[sessionId];
            return users[sessions[userId]];
        }
        throw new InvalidSessionException();
    }
    /// <summary>
    /// Logs a user out
    /// </summary>
    /// <param name="userId">sessionid</param>
    /// <returns>true</returns>
    /// <exception cref="InvalidSessionException">The session didn't exist, if it's expired it logs them out anyway</exception>
    public bool Logout(string userId)
    {
        if (sessions.ContainsKey(userId))
        {
            SessionID sessionId = sessions[userId];
            users.Remove(sessionId);
            sessions.Remove(userId);
            return true;
        }
        throw new InvalidSessionException();
    }
    /// <summary>
    /// Check if a session is valid very poorly (idk why this doesn't check lifetimes, problem for future me)
    /// TODO: Figure out why
    /// </summary>
    /// <param name="userId">session</param>
    /// <returns>If the session is valid</returns>
    public bool IsSessionValid(string userId)
    {
        return sessions.ContainsKey(userId);
    }
}

public class IncorrectPasswordException : Exception
{
}

public class InvalidSessionException : Exception
{
    
}
public class SessionExpiredException : Exception
{
}