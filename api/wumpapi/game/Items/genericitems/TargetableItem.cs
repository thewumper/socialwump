
using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;
/// <summary>
/// Targetable item, can target another player to do an effect
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
public class TargetableItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] requirements, string[] conflicts, float cooldown, TargetableItem.UseDelegate onUse)
    : CooldownItem(name, id, classType, description, price, buildTime, conflicts, requirements, cooldown), ITargetableItem
{
    private UseDelegate onUse = onUse;

    public bool Use(Player activator, Player target)
    {
        if (!IsUsable(activator)) return false;
        if (onUse.Invoke(activator, target))
        {
            base.Use(activator);
        }
        return false;
    }
    public delegate bool UseDelegate(Player activator, Player target);
}