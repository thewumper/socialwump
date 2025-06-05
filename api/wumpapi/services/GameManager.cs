using wumpapi.game;
using wumpapi.game.events;
using wumpapi.neo4j;
using wumpapi.services;
using wumpapi.structures;
using wumpapi.utils;

namespace wumpapi.Services;
/// <summary>
/// Keeps track of the current game and handles data persistence + adding players to games
/// </summary>
public class GameManager : IGameManager
{
    private Game? currentGame;
    private ILogger<IGameManager> logger;
    private IEventManager eventManager;
    private IItemRegistry itemRegistry;
    public GameManager(ILogger<IGameManager> logger, IEventManager eventManager, IItemRegistry itemRegistry)
    {
        this.logger = logger;
        this.eventManager = eventManager;
        this.itemRegistry = itemRegistry;
        eventManager.SendEvent(new TestEvent("Woot woot I test data"));
    }

    private class TestEvent(string message) : Event
    {
        public string Message { get; } = message;
    }

    public async Task Startup(INeo4jDataAccess dataAccess, IUserRepository userRepository)
    {
        bool hasSaveData = await dataAccess.ExecuteReadScalarAsync<int>(@"MATCH (n:Data) RETURN count(n)") == 1;
        var saveData = hasSaveData ? (await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Data) RETURN n {State: n.State, SavedAlliances:n.SavedAlliances} LIMIT 1","n")).FirstOrDefault()!.DictToObject<GameSaveData>() : new GameSaveData(GameState.Waiting, new List<string>());
        currentGame = new Game(saveData, logger, eventManager, itemRegistry);
        List<Player> players = new List<Player>();
        foreach (var playerData in await dataAccess.ExecuteReadDictionaryAsync(@"MATCH (n:Player) RETURN n {Username:n.Username, Items: n.Items, Alliance: n.Alliance}", "n"))
        {
            players.Add(playerData.DictToObject<PlayerData>().ToPlayer(currentGame, userRepository, itemRegistry,eventManager));
        }
        currentGame.AddSavedPlayers(players);

        if (currentGame.State == GameState.Active)
        {
            currentGame.Start();
        }
    }

    public void AutoSave(INeo4jDataAccess dataAccess, IUserRepository userRepository)
    {
        GameSaveData data = currentGame!.Save();
        string updateData = @"MERGE (d:Data) SET d += {State: $state, SavedAlliances: $alliances}";
        Dictionary<string, object> updateDataParameters = new()
        {
            {"state", data.State},
            {"alliances", data.SavedAlliances},
        };
        _ = dataAccess.ExecuteWriteTransactionAsync<bool>(updateData,updateDataParameters);

        string removePlayers = @"MATCH (n:Player) DETACH DELETE n RETURN true";
        _ = dataAccess.ExecuteWriteTransactionAsync<bool>(removePlayers);
        
        foreach (var playerData in currentGame.SavePlayers())
        {
            string createPlayer = @"CREATE (n:Player {Username: $username, Items: $items, Alliance: $alliance}) return true";
            Dictionary<string, object> createPlayerParameters = new()
            {
                {"username", playerData.Username},
                {"alliance", playerData.Alliance!},
                {"items", playerData.Items}
            };
            _ = dataAccess.ExecuteWriteTransactionAsync<bool>(createPlayer,createPlayerParameters);
        }
        
    }

    public void Shutdown(INeo4jDataAccess dataAccess, IUserRepository userRepository)
    {
        AutoSave(dataAccess, userRepository);
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

    public Player? AddPlayer(User user)
    {
        if (currentGame == null) return null;
        if (currentGame.GetPlayer(user) != null)
        {
            return currentGame.GetPlayer(user)!;
        }
        Player player = new Player(user, currentGame, eventManager);
        eventManager.SendEvent(new PlayerJoinEvent(player));
        currentGame.AddPlayer(player);
        return player;
    }

    public Game? GetCurrentGame()
    {
        return currentGame;
    }
}