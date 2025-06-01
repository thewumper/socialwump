using wumpapi.game.Items.genericitems;
using wumpapi.Services;

namespace wumpapi.game.Items;

public class ItemRegisterer
{
    public static void RegisterItems(IItemRegistry itemRegistry)
    {
        itemRegistry.RegisterItem(new Item("example", "sG_example", ItemClassType.Generic, "This is an example description", 1,5, [], []));
        
        itemRegistry.RegisterItem(new TargetableItem("Gun", "aG_gun", ItemClassType.Generic,"Receive a gun to attack others\nDeals 1p, Fires every 30 seconds", 4, 30,[],[],30,
            (activator, target) =>
            {
                // Deal damage to enemy!!!
                target.TakeDamage(activator.GetDamage());
                
                return true;
            }));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Basic Harden", "iG_harden", ItemClassType.Generic, "Increases Max Power by 5", 4, 5, [], [],
            new StatModifier(StatModifierType.Additive, new Dictionary<StatType, float>
        {
            { StatType.MaxPower , 5 }
        })));
    }
}