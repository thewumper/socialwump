using wumpapi.Services;
using wumpapi.structures;

namespace wumpapi.game;
/// <summary>
/// Keeps track of a game
/// </summary>
public class Game
{
    public Game(GameSaveData saveData)
    {
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
    
    public void AddPlayer(Player player)
    {
        players.Add(player.User, player);
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
}