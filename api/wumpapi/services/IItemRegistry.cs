using wumpapi.game.Items;

namespace wumpapi.Services;

public interface IItemRegistry
{
    void RegisterItem(IItem item);
    IItem? Parse(string itemid);
}