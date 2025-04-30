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
        var query = $"CREATE (u:User {{username: \"{user.Username}\", firstName: \"{user.FirstName}\", lastName: " +
                    $"\"{user.LastName}\", email: \"{user.Email}\", password: \"{user.Password}\"}})";
        return await neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query);
    }

    public async Task<List<Dictionary<string,object>>> FindUsers(String username)
    {
        var query = $"MATCH (u:User) WHERE toUpper(u.username) CONTAINS toUpper(${username})" +
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
}