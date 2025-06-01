using wumpapi.game;
using wumpapi.neo4j;
using wumpapi.structures;
using wumpapi.utils;

namespace wumpapi.Services;

public class GameManager : IGameManager
{
    private Game currentGame;
    
    
    public async Task Startup(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        bool hasSaveData = await dataAccess.ExecuteReadScalarAsync<int>(@"MATCH (n:Data) RETURN count(n)") == 1;
        var saveData = hasSaveData ? (await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Data) RETURN n LIMIT 1","n")).FirstOrDefault()!.DictToObject<GameSaveData>() : new GameSaveData();
        currentGame = new Game(saveData);
        List<Player> players = new List<Player>();
        foreach (var playerData in await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Player) RETURN n", "n"))
        {
            players.Add(playerData.DictToObject<PlayerData>().ToPlayer(currentGame, userRepository, itemRegistry));
        }

        currentGame.AddSavedPlayers(players);
    }

    public void AutoSave(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        throw new NotImplementedException();
    }

    public void Shutdown(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry)
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
        Player player = new Player(user, currentGame);
        currentGame.AddPlayer(player);
        return player;
    }
}