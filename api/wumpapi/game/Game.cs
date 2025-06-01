using wumpapi.game.Items;
using wumpapi.Services;
using wumpapi.structures;
using wumpapi.utils;

namespace wumpapi.game;
/// <summary>
/// Keeps track of a game
/// </summary>
public class Game
{
    private ILogger logger;
    public Game(GameSaveData saveData, ILogger logger)
    {
        this.logger = logger;
        State = saveData.State;
        foreach (var allianceName in saveData.SavedAlliances)
        {
            alliances.Add(new Alliance(allianceName));
        }
    }
    public GameState State { get; set; }

    private readonly Dictionary<User,Player> players = new();
    readonly List<Alliance> alliances = new();
    private readonly Dictionary<Player, Alliance> alliancePlayers = new();
    private List<RepeatingVariableDelayExecutor> playerUpdaters = new();

    public void Start()
    {
        State = GameState.Active;
        
        foreach (Player player in players.Values)
        {
            playerUpdaters.Add(new RepeatingVariableDelayExecutor(() =>
            {
                player.Stats.CurrentStats[StatType.Power] += player.Stats.CurrentStats[StatType.PowerGenerationAmount];
                return TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]);
            }, TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]),logger));
        }
    }

    public int WaitingPlayers()
    {
        return players.Count;
    }

    public List<RepeatingVariableDelayExecutor> GetPlayerUpdaters()
    {
        return playerUpdaters;
    }
    
    
    public void AddPlayer(Player player)
    {
        players.Add(player.User, player);
        if (players.Count >= Constants.MinimumPlayers)
        {
            Utils.RunAfterDelay(() =>
            {
                if (players.Count >= Constants.MinimumPlayers)
                    Start();
            }, Constants.TimeBetweenGames ,logger);
        }
    }

    public Alliance? GetAlliancePlayerIn(Player player)
    {
        return alliancePlayers.GetValueOrDefault(player);
    }
    
    public Player? GetPlayer(User user)
    {
        return players.GetValueOrDefault(user);
    }

    public void AddSavedPlayers(List<Player> newPlayers)
    {
        foreach (Player player in newPlayers)
        {
            AddPlayer(player);
        }
    }

    public List<Alliance> GetAllAlliances()
    {
        return alliances;
    }

    public GameSaveData Save()
    {
        return new GameSaveData(State, alliances.Select((a) => a.GetName()).ToList());
    }

    public List<PlayerData> SavePlayers()
    {
        return players.Values.Select(p => p.ToPlayerData()).ToList();
    }
}