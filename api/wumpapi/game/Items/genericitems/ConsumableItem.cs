
using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;
/// <summary>
/// ITem that can be used on yourself a limited amount of times, if time TargetedConsumableItem and ConsumableItem will not reuse code
/// TODO: SEE ABOVE
/// </summary>
/// <param name="name"></param>
/// <param name="id"></param>
/// <param name="classType"></param>
/// <param name="description"></param>
/// <param name="price"></param>
/// <param name="buildTime"></param>
/// <param name="requirements"></param>
/// <param name="conflicts"></param>
/// <param name="cooldown"></param>
/// <param name="onUse"></param>
/// <param name="maxUses"></param>
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