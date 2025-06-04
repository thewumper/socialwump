using wumpapi.game.events;
using wumpapi.game.Items;
using wumpapi.services;
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
    private IEventManager events;
    public Game(GameSaveData saveData, ILogger logger, IEventManager events)
    {
        this.logger = logger;
        this.events = events;
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
        events.SendEvent(new GameStartedEvent());
        State = GameState.Active;
        
        foreach (Player player in players.Values)
        {
            RepeatingVariableDelayExecutor updater = new RepeatingVariableDelayExecutor(() =>
            {
                if (player.Stats.Power == 0)
                {
                    // we are currently dead
                    return TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]);
                }
                player.Stats.Power += (int)float.Round(player.Stats.CurrentStats[StatType.PowerGenerationAmount]);
                if (player.Stats.Power > player.Stats.CurrentStats[StatType.MaxPower])
                {
                    player.Stats.Power = (int)float.Round(player.Stats.CurrentStats[StatType.MaxPower]);
                }
                
                events.SendEvent(new PlayerUpdatePowerEvent(player, player.Stats.Power));
                return TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]);
            }, TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]), logger);
            playerUpdaters.Add(updater);
            updater.Start();
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
        State = GameState.Waiting;
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

    public Alliance? CreateAlliance(string name, Player creator)
    {
        // Time cdrunch shopuld be dictionaiory oh well
        foreach (Alliance alliance in alliances)
        {
            if (alliance.GetName() == name)
            {
                return null;
            }
        }

        Alliance newAlliance = new Alliance(name);
        newAlliance.AddPlayer(creator);
        alliances.Add(newAlliance);
        return newAlliance;
    }
    public Alliance? JoinAlliance(string name, Player joiner)
    {
        Alliance? joined = null;
        foreach (Alliance alliance in alliances)
        {
            if (alliance.Users.Contains(joiner))
            {
                alliance.RemovePlayer(joiner);
            }
            if (alliance.GetName() == name)
            {
                alliance.AddPlayer(joiner); 
                joined = alliance;
            }
        }

        return joined;
    }

    public Alliance? LeaveAlliance(Player player)
    {
        foreach (Alliance alliance in alliances)
        {
            if (alliance.GetName() == player.AllianceName)
            {
                alliance.RemovePlayer(player);
                return alliance;
            }
        }

        return null;
    }
    

    public GameSaveData Save()
    {
        return new GameSaveData(State, alliances.Select((a) => a.GetName()).ToList());
    }

    public List<PlayerData> SavePlayers()
    {
        return players.Values.Select(p => p.ToPlayerData()).ToList();
    }

    public Graph Graph()
    {
        return new Graph(players.Values.ToArray());
    }
}