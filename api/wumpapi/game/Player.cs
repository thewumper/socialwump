using wumpapi.game.Items;
using wumpapi.structures;

namespace wumpapi.game;
/// <summary>
/// Player in a game, linked to a user
/// </summary>
public class Player(User user, Game game)
{
    public IItem?[] Items = new IItem?[Constants.ItemSlots]; 
    public User User { get; } = user;
    public Stats Stats { get; set; } = new Stats();
    public string? AllianceName { get; set; }

    private Game game = game;
    
    public PlayerData ToPlayerData()
    {
        return new PlayerData(User.Username,Items.Select(i => i.Id).ToArray(), AllianceName);
    }
    
    public int GetDamage()
    {
        float currentAttackDamage = Stats.CurrentStats[StatType.AttackPower];
        
        return (int)Math.Round(currentAttackDamage, MidpointRounding.AwayFromZero);
    }
    
    public int TakeDamage(int damage)
    {
        // Take damage with the damage resistance amount removed from the total damage
        float damageResistance = Math.Min(Stats.CurrentStats[StatType.DamageResistance], 1.0f);
        int damageTaken = (int)Math.Round((1 - damageResistance) * damage, MidpointRounding.AwayFromZero);
        
        // Add up all the absorbed damage from the other players
        Alliance? currentAlliance = game.GetAlliancePlayerIn(this);
        float totalDamageSharePercentage = 0.0f;
        if (currentAlliance != null)
        {
            foreach (Player player in currentAlliance.Users)
            {
                float playerDamageShare = player.Stats.CurrentStats[StatType.DamageShare];

                if (player == this || playerDamageShare <= 0.0f)
                {
                    continue;
                }
                
                totalDamageSharePercentage += playerDamageShare;
                
                // Take the absorbed damage from the other player
                player.Stats.CurrentStats[StatType.Power] -= (int)Math.Round(damageTaken * playerDamageShare, MidpointRounding.AwayFromZero);
            }
        }

        totalDamageSharePercentage = Math.Min(totalDamageSharePercentage, 1.0f);
        
        // Actually deal the damage
        int finalDamage =
            (int)Math.Round(damageTaken * (1 - totalDamageSharePercentage), MidpointRounding.AwayFromZero);
        Stats.CurrentStats[StatType.Power] -= finalDamage;
        return finalDamage;
    }
    
}