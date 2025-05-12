using wumpapi.structures;

namespace wumpapi.neo4j;

public class UserRepository : IUserRepository
{
    private readonly INeo4jDataAccess neo4jDataAccess;
    private readonly ILogger<UserRepository> logger;

    public UserRepository(INeo4jDataAccess neo4jDataAccess, ILogger<UserRepository> logger)
    {
        this.neo4jDataAccess = neo4jDataAccess;
        this.logger = logger;
    }
    
    public async Task<bool> AddUser(User user)
    {
        var query = @"CREATE (u:User {Username: $Username, Firstname: $Firstname, Lastname: $Lastname, Email: $Email, Password: $Password}) RETURN true";
        IDictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "Username", user.Username },
            { "Firstname", user.FirstName },
            { "Lastname", user.LastName },
            { "Email", user.Email },
            { "Password", user.Password }
        };
        return await neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query,parameters);
    }

    public async Task<List<Dictionary<string,object>>> FindUsers(String username)
    {
        var query = $"MATCH (u:User) WHERE toUpper(u.Username) CONTAINS toUpper(${username})" +
                    $"RETURN u ORDER BY u.Username";
        return await neo4jDataAccess.ExecuteReadDictionaryAsync(query,"u");
    }
    
    public async Task<long> GetUserCount()
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetUsers()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExists(string requestUsername, string requestEmail)
    {
        throw new NotImplementedException();
    }
}