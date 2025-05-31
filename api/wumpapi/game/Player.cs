using wumpapi.game.Items;
using wumpapi.structures;

namespace wumpapi.game;
/// <summary>
/// Player in a game, linked to a user
/// </summary>
public class Player(User user)
{
    public const int ItemSlots = 5;
    public readonly IItem?[] Items = new IItem?[5]; 
    public User User { get; } = user;
    public Stats Stats { get; set; } = new Stats();
    
}