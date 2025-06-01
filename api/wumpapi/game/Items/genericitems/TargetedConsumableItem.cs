using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;

public class TargetedConsumableItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] conflicts, string[] requirements, float cooldown, TargetableItem.UseDelegate onUse, int maxUses) 
    : TargetableItem(name, id, classType, description, price, buildTime, conflicts, requirements, cooldown, onUse), ITargetedConsumableItem
{
    public new bool Use(Player activator, Player target)
    {
        if (RemainingUses - 1 < 0)
        {
            return false;// Uh oh
        }
        if (base.Use(activator, target))
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