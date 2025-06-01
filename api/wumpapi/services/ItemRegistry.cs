using System.Text.Json;
using wumpapi.game.Items;
using wumpapi.game.Items.genericitems;
using wumpapi.utils;

namespace wumpapi.Services;

public class ItemRegistry : IItemRegistry
{
    readonly Dictionary<string, IItem> items = new();
    public void RegisterItem(IItem item)
    {
        items.Add(item.Id, item);
    }
    public IItem? Parse(string itemid)
    {
        return items.TryGetValue(itemid, out var item) ? DeepCopyUtils.DeepCopy(item) : null;
    }

    public List<IItem> GetItems()
    {
        return items.Values.ToList();
    }

    public IItem GetItem(string itemid)
    {
        return items[itemid];
    }
}