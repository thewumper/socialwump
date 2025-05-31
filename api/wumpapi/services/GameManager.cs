using wumpapi.game;
using wumpapi.neo4j;
using wumpapi.structures;
using wumpapi.utils;

namespace wumpapi.Services;

public class GameManager : IGameManager
{
    private Game currentGame;
    
    
    public async Task Startup(INeo4jDataAccess dataAccess)
    {
        bool hasSaveData = await dataAccess.ExecuteReadScalarAsync<int>(@"MATCH (n:Data) RETURN count(n)") == 1;
        var saveData = hasSaveData ? (await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Data) RETURN n LIMIT 1","n")).FirstOrDefault()!.DictToObject<GameSaveData>() : new GameSaveData();
        List<Player> players = new List<Player>();
        foreach (var playerData in await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Player) RETURN n", "n"))
        {
            // TODO:FINISH
            //players.Add(playerData.DictToObject<PlayerData>().ToPlayer());
        }
        // TODO: Finish
        //currentGame = new Game(saveData,players);
        
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
        if (currentGame.State == GameState.Active)
        {
            return currentGame;
        }
        throw new InvalidOperationException("The game is not active.");
    }

    public GameState GetGameState()
    {
        return currentGame.State;
    }

    public Player AddPlayer(User user)
    {
        Player player = new Player(user);
        currentGame.AddPlayer(player);
        return player;
    }
}