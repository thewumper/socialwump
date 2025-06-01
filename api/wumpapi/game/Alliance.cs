
namespace wumpapi.game;
/// <summary>
/// Named group of players. No fees (not implemented yet) + wins together
/// </summary>
/// <param name="name"></param>
public class Alliance(string name)
{
    private readonly HashSet<Player> users = new();

    
    public HashSet<Player> Users => users;
    
    public string GetName()
    {
        return name;
    }

    public void AddPlayer(Player user)
    {
        users.Add(user);
        user.AllianceName = name;
    }

    public void RemovePlayer(Player user)
    {
        users.Remove(user);
        user.AllianceName = null;
    }
}