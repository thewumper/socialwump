
namespace wumpapi.game;
/// <summary>
/// Named group of players. No fees (not implemented yet) + wins together
/// </summary>
/// <param name="name"></param>
public class Alliance(string name)
{
    private readonly HashSet<Player> players = new();

    
    public HashSet<Player> Players => players;
    
    public string GetName()
    {
        return name;
    }

    public void AddPlayer(Player user)
    {
        players.Add(user);
        user.AllianceName = name;
    }

    public void RemovePlayer(Player user)
    {
        players.Remove(user);
        user.AllianceName = null;
    }
}