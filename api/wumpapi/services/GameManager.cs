using wumpapi.game;
using wumpapi.neo4j;
using wumpapi.structures;

namespace wumpapi.Services;

public class GameManager : IGameManager
{
    private Game currentGame;
    
    
    
    public void Startup(INeo4jDataAccess dataAccess)
    {
        throw new NotImplementedException();
    }

    public void AutoSave(INeo4jDataAccess dataAccess)
    {
        throw new NotImplementedException();
    }

    public void Shutdown(INeo4jDataAccess dataAccess)
    {
        throw new NotImplementedException();
    }

    public Game GetActiveGame()
    {
        throw new NotImplementedException();
    }

    public GameState GetGameState()
    {
        throw new NotImplementedException();
    }

    public Player AddPlayer(User user)
    {
        throw new NotImplementedException();
    }

    public void LeavePlayer(User user)
    {
        throw new NotImplementedException();
    }
}