using wumpapi.game.Items.genericitems;
using wumpapi.Services;

namespace wumpapi.game.Items;

public class ItemRegisterer
{
    public static void RegisterItems(IItemRegistry itemRegistry)
    {
        itemRegistry.RegisterItem(new Item("example", "EXAMPLE", "This is an example description", 1,5, [], []));
        
        itemRegistry.RegisterItem(new TargetableItem("Gun", "aG_gun","Receive a gun to attack others\nDeals 1p, Fires every 30 seconds", 4, 30,[],[],30,
            (activator, target) =>
            {
                // Deal damage to enemy!!!
                target.TakeDamage(activator.GetDamage());
                
                return true;
            }));
    }
}