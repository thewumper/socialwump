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
    public Task Startup(INeo4jDataAccess dataAccess, IUserRepository userRepository);
    public void AutoSave(INeo4jDataAccess dataAccess, IUserRepository userRepository);
    public void Shutdown(INeo4jDataAccess dataAccess, IUserRepository userRepository);
    public Game GetActiveGame();
    public GameState? GetGameState();
    public Player? AddPlayer(User user);
    Game? GetCurrentGame();
}

public enum GameState
{
    Active,
    Waiting,
    Starting,
}