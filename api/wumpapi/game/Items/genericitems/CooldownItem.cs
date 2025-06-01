using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;

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