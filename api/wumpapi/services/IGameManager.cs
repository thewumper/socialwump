using wumpapi.game;
using wumpapi.neo4j;
using wumpapi.services;
using wumpapi.structures;

namespace wumpapi.Services;
/// <summary>
/// Keeps track of the current game and handles data persistence + adding players to games
/// </summary>
public interface IGameManager
{
    public Task Startup(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry, IEventManager eventManager);
    public void AutoSave(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry);
    public void Shutdown(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry);
    public Game GetActiveGame();
    public GameState? GetGameState();
    public Player? AddPlayer(User user, IEventManager eventManager);
    Game? GetCurrentGame();
}

public enum GameState
{
    Active,
    Waiting,
    Starting,
}