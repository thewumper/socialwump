using Microsoft.AspNetCore.Identity;
using wumpapi.neo4j;
using wumpapi.structures;

namespace wumpapi.Services;

public class SessionManager : ISessionManager
{
    private readonly ILogger<SessionManager> logger;
    private readonly UserRepository userRepository;
    private readonly PasswordHasher<User> passwordHasher;

    private Dictionary<SessionID, User> users;

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
            users.Add(sessionId, user);
            return new Tuple<string,User>(sessionId.SSID, user);
        }
        else
        {
            throw new IncorrectPasswordException();
        }
    }

    public async Task<User> GetAuthedUser(string userId)
    {
        if ()
        return users[userId];
    }

    public async Task<bool> Logout(string userId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> IsValidAuth(string userId)
    {
        throw new NotImplementedException();
    }
}

public class IncorrectPasswordException : Exception
{
}