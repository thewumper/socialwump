using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;

public class UsableItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] conflicts, string[] requirements, float cooldown, UsableItem.UseDelegate onUse) 
    : CooldownItem(name, id, classType, description, price, buildTime, conflicts, requirements, cooldown), IUsableItem
{
    public new bool Use(Player activator)
    {
        if (!IsUsable(activator)) return false;
        if (onUse.Invoke(activator))
        {
            base.Use(activator);
        }
        return false;
    }
    public delegate bool UseDelegate(Player activator);
}