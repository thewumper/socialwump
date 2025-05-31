using wumpapi.game;
using wumpapi.neo4j;
using wumpapi.structures;

namespace wumpapi.Services;

public interface IGameManager
{
    public Task Startup(INeo4jDataAccess dataAccess);
    public void AutoSave(INeo4jDataAccess dataAccess);
    public void Shutdown(INeo4jDataAccess dataAccess);
    public Game GetActiveGame();
    public GameState GetGameState();
    public Player AddPlayer(User user);
}

public enum GameState
{
    Active,
    Waiting,
    Starting,
}