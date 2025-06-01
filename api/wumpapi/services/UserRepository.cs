using wumpapi.structures;
using wumpapi.utils;

namespace wumpapi.neo4j;
/// <summary>
/// Service for querying neo4j about users and their connections
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly INeo4jDataAccess neo4JDataAccess;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<UserRepository> logger;

    public UserRepository(INeo4jDataAccess neo4JDataAccess, ILogger<UserRepository> logger)
    {
        this.neo4JDataAccess = neo4JDataAccess;
        this.logger = logger;
    }
    /// <summary>
    /// Add a user to the database
    /// </summary>
    /// <param name="user">user to add</param>
    /// <returns>true</returns>
    public async Task<bool> AddUser(User user)
    {
        var query = @"CREATE (u:User {Username: $Username, Firstname: $Firstname, Lastname: $Lastname, Email: $Email, Password: $Password}) RETURN true";
        IDictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "Username", user.Username },
            { "Firstname", user.Firstname },
            { "Lastname", user.Lastname },
            { "Email", user.Email },
            { "Password", user.Password! }
        };
        return await neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query,parameters);
    }

    public async Task<List<Dictionary<string,object>>> FindUsers(String username)
    {
        var query = $"MATCH (u:User) WHERE toUpper(u.Username) CONTAINS toUpper(${username})" +
                    $"RETURN u ORDER BY u.Username";
        return await neo4JDataAccess.ExecuteReadDictionaryAsync(query,"u");
    }
    /// <summary>
    /// Not finished
    /// </summary>
    public async Task<long> GetUserCount()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get every user
    /// </summary>
    /// <returns>The users</returns>
    public async Task<List<User>> GetUsers()
    {
        var query = @"MATCH (n:User) RETURN n {Username:n.Username, Firstname:n.Firstname, Lastname:n.Lastname, Email:n.Email, Password:null}";
        List<User> users = new List<User>();
        foreach (Dictionary<string,object> nodes in await neo4JDataAccess.ExecuteReadDictionaryAsync(query,"n"))
        {
            users.Add(nodes.DictToObject<User>());
        }
        return users;
    }
    
    private async Task<List<Connection>> GetConnections()
    {
        var query = @"MATCH (i:User)-[r]->(t:User) RETURN {Initiator: {Firstname: i.Firstname, Email: i.Email, Password: null, Username: i.Username, Lastname: i.Lastname}, Recipient : {Firstname: t.Firstname, Email: t.Email, Password: null, Username: t.Username, Lastname: t.Lastname}, Name:type(r), Data: r.Data} as r ";
        List<Connection> connections = new List<Connection>();
        foreach (Dictionary<string,object> relationship in
                 await neo4JDataAccess.ExecuteReadDictionaryAsync(query, "r"))
        {
            connections.Add(relationship.DictToObject<Connection>());
        }
        return connections;
    }
    /// <summary>
    /// Checks if a user exists with the given username OR email
    /// </summary>
    /// <param name="requestUsername">username to check against</param>
    /// <param name="requestEmail">email to check against</param>
    /// <returns>if the user exists or not</returns>
    public async Task<bool> UserExists(string requestUsername, string requestEmail)
    {
        var query = @"RETURN (EXISTS { MATCH (u:User) WHERE toLower(u.Username) = toLower($Username) } OR EXISTS { MATCH (u:User) WHERE toLower(u.Email) = toLower($Email) }) AS Predicate";
        IDictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "Username", requestUsername },
            { "Email", requestEmail }
        };
        return await neo4JDataAccess.ExecuteReadScalarAsync<bool>(query, parameters);
    }
    /// <summary>
    /// Get a user that matches a username email or both
    /// </summary>
    /// <remarks>Maybe this should be two functions?</remarks>>
    /// <param name="username">Leave empty to check just email</param>
    /// <param name="email">leave empty to check just for username</param>
    /// <param name="includePassword">Censor password?</param>
    /// <returns>A user matching either the username email or both</returns>
    public async Task<User> GetUser(string username="", string email="",bool includePassword=false)
    {
        if (username == "" && email == "")
        {
            throw new ArgumentException($"Both {username} and {email} cannot be empty");
        }

        string query;
        Dictionary<string,object> parameters;
        // This seems awful
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
        return ((await neo4JDataAccess.ExecuteReadDictionaryAsync(query, "u",parameters)).FirstOrDefault() ?? throw new UserNotFoundException()).DictToObject<User>();

    }
    /// <summary>
    /// Create a connection between nodes
    /// </summary>
    /// <param name="initiator">Source of the connection</param>
    /// <param name="target">Target of the connection</param>
    /// <param name="name">Name of the relationship</param>
    /// <param name="data">Data of the relationship, may be replaced by some object later</param>
    /// <returns>Did it work?</returns>
    public async Task<bool> CreateRelationship(User initiator, User target, string name, string data)
    {
        string query = @"MATCH (i:User) WHERE i.Username=$iUsername MATCH (t:User) WHERE t.Username=$tUsername CREATE (i)-[:relation {Data:$data}]->(t) RETURN TRUE";
        // TODO: This leaves a security vulnerability but this is just to get things to work (direct replacement)
        query = query.Replace("relation", name);
        IDictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "iUsername", initiator.Username },
            { "tUsername", target.Username },
            { "data", data }
        };
        return await neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
    }


}


public class UserNotFoundException : Exception
{
}