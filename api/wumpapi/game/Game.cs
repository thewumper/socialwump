using wumpapi.Services;
using wumpapi.structures;

namespace wumpapi.game;
/// <summary>
/// Keeps track of a game
/// </summary>
public class Game(GameSaveData saveData)
{
    public GameState State { get; set; }
    
    Dictionary<User,Player> players = new();
    List<Alliance> alliances = new();
    Dictionary<Player, Alliance> alliancePlayers = new();
    GameSaveData saveData = saveData;
    
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

    public void AddSavedPlayers(List<Player> list)
    {
    }
}