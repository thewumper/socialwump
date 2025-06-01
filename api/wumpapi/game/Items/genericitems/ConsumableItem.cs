
namespace wumpapi.game.Items.genericitems;
public class ConsumableItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] requirements, string[] conflicts, float cooldown, UsableItem.UseDelegate onUse, int maxUses)
    : UsableItem(name, id, classType, description, price, buildTime, conflicts, requirements, cooldown, onUse), IConsumableItem
{
    public new bool Use(Player activator)
    {
        if (RemainingUses - 1 < 0)
        {
            return false;// Uh oh
        }
        if (base.Use(activator))
        {
            RemainingUses--;
            if (RemainingUses != 0) return true;
            for (var i = 0; i < activator.Items.Length; i++)
            {
                var item = activator.Items[i];
                if (item == this)
                {
                    activator.Items[i] = null;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public int RemainingUses { get; private set;  } = maxUses;
}