using wumpapi.structures;

namespace wumpapi.neo4j;

public interface IUserRepository
{ 
    Task<User> AddUser(User user);
    Task<long> GetUserCount();
    Task<List<User>> GetUsers();
}