using wumpapi.structures;

namespace wumpapi.game;

public class Alliance(string name)
{
    private readonly string name = name;
    private readonly HashSet<User> users = new();

    public string GetName()
    {
        return name;
    }

    public void AddUser(User user)
    {
        users.Add(user);
    }
}