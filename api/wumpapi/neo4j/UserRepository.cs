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
    
    public async Task<User> AddUser(User user)
    {
        var query = $"CREATE (u:User {{Username: \"{user.Username}\", FirstName: \"{user.FirstName}\", LastName: " +
                    $"\"{user.LastName}\", Email: \"{user.Email}\", Password: \"{user.Password}\"}})"
                    + "RETURN u";
        return await neo4jDataAccess.ExecuteWriteTransactionAsync<User>(query);
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
}