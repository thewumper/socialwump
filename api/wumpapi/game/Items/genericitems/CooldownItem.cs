using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;
/// <summary>
/// Item that has a cooldown which can be queried and reset, no effects though
/// </summary>
/// <param name="name"></param>
/// <param name="id"></param>
/// <param name="classType"></param>
/// <param name="description"></param>
/// <param name="price"></param>
/// <param name="buildTime"></param>
/// <param name="conflicts"></param>
/// <param name="requirements"></param>
/// <param name="baseCooldown"></param>
public class CooldownItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] conflicts, string[] requirements, float baseCooldown) : Item(name, id, classType, description, price, buildTime, conflicts, requirements), ICooldownItem
{
    public float BaseCooldown { get; protected set; } = baseCooldown;
    public DateTime LastUsed { get; private set; }
    public float Cooldown(Player player)
    {
        return BaseCooldown * (1 - player.Stats.CurrentStats[StatType.CooldownReduction]);
    }
    public bool IsUsable(Player player)
    {
        return LastUsed.AddSeconds(Cooldown(player)) <= DateTime.Now;
    }
    public bool Use(Player player)
    {
        if (!IsUsable(player)) return false;
        LastUsed = DateTime.Now;
        return true;
    }
    
}