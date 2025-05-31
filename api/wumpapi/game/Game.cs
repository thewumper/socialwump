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
    GameSaveData saveData = saveData;
    
    public void AddPlayer(Player player)
    {
        players.Add(player.User, player);
    }

    public Player GetPlayer(User user)
    {
        throw new NotImplementedException();
    }
}