using wumpapi.structures;

namespace wumpapi.neo4j;
/// <summary>
/// Allows querying of the neo4j database
/// </summary>
public interface IUserRepository
{ 
    /// <summary>
    /// Add a user to the database
    /// </summary>
    /// <param name="user">user to add</param>
    /// <returns>true if successful, exception otherwise, this probably can be void idk</returns>
    /// TODO: Can this and other bool methods become void
    Task<bool> AddUser(User user);
    /// <summary>
    /// How many users are there>
    /// </summary>
    /// <returns>Number of users</returns>
    Task<long> GetUserCount();
    /// <summary>
    /// Gets every single user with passwords censored
    /// </summary>
    /// <returns>List of users</returns>
    Task<List<User>> GetUsers();
    /// <summary>
    /// Checks if a user exists with the given username OR email
    /// </summary>
    /// <param name="requestUsername">username to check against</param>
    /// <param name="requestEmail">email to check against</param>
    /// <returns>if the user exists or not</returns>
    Task<bool> UserExists(string requestUsername, string requestEmail);
    /// <summary>
    /// Get a user that matches a username email or both
    /// </summary>
    /// <param name="username">Leave empty to check just email</param>
    /// <param name="email">leave empty to check just for username</param>
    /// <param name="includePassword">Censor password?</param>
    /// <returns>A user matching either the username email or both</returns>
    Task<User> GetUser(string username="",string email="",bool includePassword = false);
    /// <summary>
    /// Create a connection between nodes
    /// </summary>
    /// <param name="initiator">Source of the connection</param>
    /// <param name="target">Target of the connection</param>
    /// <param name="name">Name of the relationship</param>
    /// <param name="data">Data of the relationship, may be replaced by some object later</param>
    /// <returns>Did it work?</returns>
    /// TODO: also funky
    Task<bool> CreateRelationship(User initiator, User target, string name, string data);
    /// <summary>
    /// Get a max readable graph for sending
    /// </summary>
    /// <returns>max readable graph</returns>
    Task<Graph> GetGraph();
}