using System.Text.Json;
using wumpapi.game.Items;
using wumpapi.game.Items.genericitems;
using wumpapi.utils;

namespace wumpapi.Services;
/// <summary>
/// Service for storing items
/// </summary>
public class ItemRegistry : IItemRegistry
{
    readonly Dictionary<string, IItem> registeredItems = new();
    readonly HashSet<string> winIds = new();
    public IItem RegisterItem(IItem item)
    {
        registeredItems.Add(item.Id, item);
        return item;
    }
    public IItem? Parse(string itemid)
    {
        return registeredItems.TryGetValue(itemid, out var item) ? DeepCopyUtils.DeepCopy(item) : null;
    }

    public List<IItem> GetItems()
    {
        return registeredItems.Values.ToList();
    }

    
    public void AddToWin(IItem item)
    {
        winIds.Add(item.Id);
    }

    public bool Wins(IItem[] items)
    {
        if (items.Length != winIds.Count) return false;
        return items.All(item => winIds.Contains(item.Id));
    }

    public bool IsWinItem(IItem item)
    {
        return winIds.Contains(item.Id);
    }
}