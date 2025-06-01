using wumpapi.game;
using wumpapi.neo4j;
using wumpapi.structures;
using wumpapi.utils;

namespace wumpapi.Services;

public class GameManager : IGameManager
{
    private Game? currentGame;
    private ILogger<IGameManager> logger;

    public GameManager(ILogger<IGameManager> logger)
    {
        this.logger = logger;
    }
    
    public async Task Startup(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        bool hasSaveData = await dataAccess.ExecuteReadScalarAsync<int>(@"MATCH (n:Data) RETURN count(n)") == 1;
        var saveData = hasSaveData ? (await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Data) RETURN n {State: n.State, SavedAlliances:n.SavedAlliances} LIMIT 1","n")).FirstOrDefault()!.DictToObject<GameSaveData>() : new GameSaveData(GameState.Waiting, new List<string>());
        currentGame = new Game(saveData, logger);
        List<Player> players = new List<Player>();
        foreach (var playerData in await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Player) RETURN n {Username:n.Username, Items: n.Items, Alliance: n.Alliance}", "n"))
        {
            players.Add(playerData.DictToObject<PlayerData>().ToPlayer(currentGame, userRepository, itemRegistry));
        }
        currentGame.AddSavedPlayers(players);

        if (currentGame.State == GameState.Active)
        {
            currentGame.Start();
        }
    }

    public void AutoSave(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        GameSaveData data = currentGame.Save();
        string updateData = @"MERGE (d:Data) SET d += {State: $state, SavedAlliances: $alliances}";
        Dictionary<string, object> updateDataParameters = new()
        {
            {"state", data.State},
            {"alliances", data.SavedAlliances},
        };
        dataAccess.ExecuteWriteTransactionAsync<bool>(updateData,updateDataParameters);

        string removePlayers = @"MATCH (n:Player) DETACH DELETE n RETURN true";
        dataAccess.ExecuteWriteTransactionAsync<bool>(removePlayers);
        
        foreach (var playerData in currentGame.SavePlayers())
        {
            string createPlayer = @"CREATE (n:Player {Username: $username, Items: $items, Alliance: $alliance}) return true";
            Dictionary<string, object> createPlayerParameters = new()
            {
                {"username", playerData.Username},
                {"alliances", playerData.Alliance!},
                {"items", playerData.Items}
            };
            dataAccess.ExecuteWriteTransactionAsync<bool>(createPlayer,createPlayerParameters);
        }
        
    }

    public void Shutdown(INeo4jDataAccess dataAccess, IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        AutoSave(dataAccess, userRepository, itemRegistry);
        currentGame?.GetPlayerUpdaters().ForEach(updater => updater.Stop());
        currentGame = null;
    }

    public Game GetActiveGame()
    {
        if (currentGame is { State: GameState.Active })
        {
            return currentGame;
        }
        throw new InvalidOperationException("The game is not active.");
    }

    public GameState? GetGameState()
    {
        return currentGame?.State;
    }

    public Player AddPlayer(User user)
    {
        if (currentGame == null) return null;
        if (currentGame.GetPlayer(user) != null)
        {
            return currentGame.GetPlayer(user)!;
        }
        Player player = new Player(user, currentGame);
        currentGame.AddPlayer(player);
        return player;
    }

    public Game? GetCurrentGame()
    {
        return currentGame;
    }
}