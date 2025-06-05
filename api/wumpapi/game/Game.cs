using wumpapi.game.events;
using wumpapi.game.Items;
using wumpapi.game.Items.genericitems;
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
    private readonly IEventManager events;
    private readonly IItemRegistry itemRegistry;
    public Game(GameSaveData saveData, ILogger logger, IEventManager events, IItemRegistry itemRegistry)
    {
        this.logger = logger;
        this.events = events;
        this.itemRegistry = itemRegistry;
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
            RepeatingVariableDelayExecutor updater = new RepeatingVariableDelayExecutor(Task<TimeSpan> () =>
            {
                if (player.Stats.Power == 0)
                {
                    // we are currently dead
                    return Task.FromResult(TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]));
                }
                player.Stats.Power += (int)float.Round(player.Stats.CurrentStats[StatType.PowerGenerationAmount]);
                if (player.Stats.Power > player.Stats.CurrentStats[StatType.MaxPower])
                {
                    player.Stats.Power = (int)float.Round(player.Stats.CurrentStats[StatType.MaxPower]);
                }
                
                events.SendEvent(new PlayerUpdatePowerEvent(player, player.Stats.Power));
                return Task.FromResult(TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]));
            }, TimeSpan.FromSeconds(player.Stats.CurrentStats[StatType.PowerGenerationPeriod]), logger);
            playerUpdaters.Add(updater);
            updater.Start();
        }
        events.Subscribe<PlayerFinishMakingItemEvent>(CheckIfTeamWon);
        events.SendEvent(new GameStartedEvent());
    }

    void CheckIfTeamWon(PlayerFinishMakingItemEvent e)
    {
        if (!itemRegistry.IsWinItem(e.Item)) return;
        Alliance alliance = alliancePlayers[e.Player];
        List<IItem> winItemsInAlliance = [];
        foreach (Player player in alliance.Players)
        {
            foreach (IItem? playerItem in player.Items)
            {
                if (playerItem == null) continue;
                if (itemRegistry.IsWinItem(playerItem))
                {
                    winItemsInAlliance.Add(playerItem);
                }
            }
        }

        if (itemRegistry.Wins(winItemsInAlliance.ToArray()))
        {
            events.SendEvent(new AllianceWinEvent(alliance)); //
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

    private bool isStarting = false;
    public void AddPlayer(Player player)
    {
        
        players.Add(player.User, player);
        if (players.Count >= Constants.MinimumPlayers && !isStarting)
        {
            isStarting = true;
            Utils.RunAfterDelay(() =>
            {
                if (players.Count >= Constants.MinimumPlayers)
                    Start();
                isStarting = false;

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
            if (alliance.Players.Contains(joiner))
            {
                alliance.RemovePlayer(joiner);
            }
            if (alliance.GetName() == name)
            {
                alliancePlayers[joiner] = alliance;
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
                alliancePlayers.Remove(player);
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