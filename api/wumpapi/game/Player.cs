using wumpapi.structures;

namespace wumpapi.game;
/// <summary>
/// Player in a game, linked to a user
/// </summary>
public class Player(User User)
{
    List<Item> items = [];
    private int power;
    public User User { get; }
}