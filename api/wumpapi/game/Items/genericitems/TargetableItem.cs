using MemoryPack;

namespace wumpapi.game.Items.genericitems;
public class TargetableItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] requirements, string[] conflicts, float cooldown, TargetableItem.UseDelegate onUse)
    : Item(name, id, classType, description, price, buildTime, conflicts, requirements), ITargetableItem
{
    public float Cooldown { get; } = cooldown;
    private DateTime lastUse;
    private UseDelegate onUse = onUse;
    public bool Use(Player activator, Player target)
    {
        if (lastUse.AddSeconds(Cooldown) > DateTime.Now) return false;
        
        lastUse = DateTime.Now;
        return onUse.Invoke(activator, target);

    }
    public delegate bool UseDelegate(Player activator, Player target);
}