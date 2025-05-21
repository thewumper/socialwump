using System.Collections;
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
            users.Add(nodes.DictToObject<User>());
        }
        return users;
    }
    private async Task<List<Connection>> GetConnections()
    {
        var query = @"MATCH (i:User)-[r]->(t:User) RETURN {Initiator: {Firstname: i.Firstname, Email: i.Email, Password: null, Username: i.Username, Lastname: i.Lastname}, Recipient : {Firstname: t.Firstname, Email: t.Email, Password: null, Username: t.Username, Lastname: t.Lastname}, Name:type(r), Data: r.Data} as r ";
        List<Connection> connections = new List<Connection>();
        foreach (Dictionary<string,object> relationship in
                 await neo4jDataAccess.ExecuteReadDictionaryAsync(query, "r"))
        {
            connections.Add(relationship.DictToObject<Connection>());
        }
        return connections;
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
    
    public async Task<bool> CreateRelationship(User initiator, User target, string requestRelationshipName, string requestData)
    {
        // TODO: This leaves a security vulnerability but this is just to get things to work (direct replacement)
        string query = @"MATCH (i:User) WHERE i.Username=$iUsername MATCH (t:User) WHERE t.Username=$tUsername CREATE (i)-[:relation {Data:$data}]->(t) RETURN TRUE";
        query = query.Replace("relation", requestRelationshipName);
        IDictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "iUsername", initiator.Username },
            { "tUsername", target.Username },
            { "data", requestData }
        };
        return await neo4jDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
    }

    public async Task<Graph> GetGraph()
    {
        User[] users = (await GetUsers()).ToArray();
        Connection[] connections = (await GetConnections()).ToArray();
        
        return new Graph(users, connections);
    }


}


public class UserNotFoundException : Exception
{
}