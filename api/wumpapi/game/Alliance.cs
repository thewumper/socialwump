
namespace wumpapi.game;

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
}