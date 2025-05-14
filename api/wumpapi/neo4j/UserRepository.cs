using wumpapi.structures;
using wumpapi.utils;

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
            { "Firstname", user.Firstname },
            { "Lastname", user.Lastname },
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
        var query = @"MATCH (n:User) RETURN n {Username:n.Username, Firstname:n.Firstname, Lastname:n.Lastname, Email:n.Email, Password:null}";
        List<User> users = new List<User>();
        foreach (Dictionary<string,object> nodes in await neo4jDataAccess.ExecuteReadDictionaryAsync(query,"n"))
        {
            logger.LogError(string.Join(",",nodes));
            users.Add(nodes.DictToObject<User>());
        }
        return users;
    }

    public async Task<bool> UserExists(string requestUsername, string requestEmail)
    {
        var query = @"RETURN (EXISTS { MATCH (u:User) WHERE toLower(u.Username) = toLower($Username) } OR EXISTS { MATCH (u:User) WHERE toLower(u.Email) = toLower($Email) }) AS Predicate";
        IDictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "Username", requestUsername },
            { "Email", requestEmail }
        };
        return await neo4jDataAccess.ExecuteReadScalarAsync<bool>(query, parameters);
    }
    // Should this be two functions?
    public async Task<User> GetUser(string username="", string email="",bool includePassword=false)
    {
        if (username == "" && email == "")
        {
            throw new ArgumentException($"Both {username} and {email} cannot be empty");
        }

        string query;
        Dictionary<string,object> parameters;
        if (email == "")
        {
            query = @"MATCH (u:User) WHERE toUpper(u.Username)=toUpper($Username)";
            parameters = new Dictionary<string, object>()
            {
                { "Username", username },
            };
        }
        else if (username == "")
        {
            query = @"MATCH (u:User) WHERE toUpper(u.Email)=toUpper($Email)";
            parameters = new Dictionary<string, object>()
            {
                { "Email", email },
            };
        }
        else
        {
            query = @"MATCH (u:User) WHERE (toUpper(u.Username)=toUpper($Username)) AND toUpper(u.Email)=toUpper($Email))";
            parameters = new Dictionary<string, object>()
            {
                { "Username", username },
                { "Email", email },
            };
        }
        query +=
            $" RETURN u {{Username:u.Username, Firstname:u.Firstname, Lastname:u.Lastname, Email:u.Email, {(includePassword ? "Password:u.Password" : "Password:null")}}}";
        return ((await neo4jDataAccess.ExecuteReadDictionaryAsync(query, "u",parameters)).FirstOrDefault() ?? throw new UserNotFoundException()).DictToObject<User>();

    }
}

public class UserNotFoundException : Exception
{
}