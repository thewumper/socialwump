namespace wumpapi.game.Items.genericitems;

public class UsableItem(string name, string id, string description, int price, int buildTime, string[] conflicts, string[] requirements, float cooldown, UsableItem.UseDelegate onUse) 
    : Item(name, id, description, price, buildTime, conflicts, requirements), IUsableItem
{
    public float Cooldown { get; } = cooldown;
    private DateTime lastUse;
    public bool Use(Player activator)
    {
        if (lastUse.AddSeconds(Cooldown) > DateTime.Now) return false;
        
        lastUse = DateTime.Now;
        return onUse.Invoke(activator);

    }
    public delegate bool UseDelegate(Player activator);
}