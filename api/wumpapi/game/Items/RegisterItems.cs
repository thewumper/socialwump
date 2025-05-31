using wumpapi.game.Items.genericitems;
using wumpapi.Services;

namespace wumpapi.game.Items;

public class ItemRegisterer
{
    public static void RegisterItems(IItemRegistry itemRegistry)
    {
        itemRegistry.RegisterItem(new Item("example", "EXAMPLE", "example description", 1, [], []));
    }
}