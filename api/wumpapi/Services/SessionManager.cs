using Microsoft.AspNetCore.Identity;
using Neo4j.Driver;
using wumpapi.neo4j;
using wumpapi.structures;

namespace wumpapi.Services;

public class SessionManager : ISessionManager
{
    private readonly ILogger<SessionManager> logger;
    private readonly UserRepository userRepository;
    private readonly PasswordHasher<User> passwordHasher;

    private Dictionary<SessionID, User> users;
    private Dictionary<string, SessionID> sessions;
    public SessionManager(UserRepository userRepository, PasswordHasher<User> passwordHasher, ILogger<SessionManager> logger)
    {
        this.userRepository = userRepository;
        this.logger = logger;
        this.passwordHasher = passwordHasher;
        users = new Dictionary<SessionID, User>();
    }

    public async Task<Tuple<string,User>> AuthUser(String username, String password)
    {
        User user = await userRepository.GetUser(username);
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Success)
        {
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