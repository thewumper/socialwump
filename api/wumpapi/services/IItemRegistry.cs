using wumpapi.game.Items;

namespace wumpapi.Services;
/// <summary>
/// Service for storing items
/// </summary>
public interface IItemRegistry
{
    void RegisterItem(IItem item);
    IItem? Parse(string itemid);
    List<IItem> GetItems();
    IItem GetItem(string itemid);
}