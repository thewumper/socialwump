using wumpapi.game.Items.genericitems;
using wumpapi.game.Items.interfaces;

namespace wumpapi.utils;
/// <summary>
/// Utility class for items
/// </summary>
public static class Items
{
    public static bool IsPositive(IItem item)
    {
        return item is IPositiveItem;
    }
}