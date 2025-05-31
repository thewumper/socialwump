using wumpapi.game.Items;
using wumpapi.structures;

namespace wumpapi.game;
/// <summary>
/// Player in a game, linked to a user
/// </summary>
public class Player(User user, Game game)
{
    public const int ItemSlots = 5;
    public readonly IItem?[] Items = new IItem?[5]; 
    public User User { get; } = user;
    public Stats Stats { get; set; } = new Stats();

    private Game game = game;
    
    public int GetDamage()
    {
        float currentAttackDamage = Stats.CurrentStats[StatType.AttackPower];
        
        return (int)Math.Round(currentAttackDamage, MidpointRounding.AwayFromZero);
    }
    
    public void TakeDamage(int damage)
    {
        float damageResistance = Stats.CurrentStats[StatType.DamageResistance];
        
        // Take damage with the damage resistance amount removed from the total damage
        Stats.CurrentStats[StatType.Power] -= (1 - damageResistance) * damage;
    }
    
}