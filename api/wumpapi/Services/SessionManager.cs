using Microsoft.AspNetCore.Identity;
using Neo4j.Driver;
using wumpapi.neo4j;
using wumpapi.structures;

namespace wumpapi.Services;

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