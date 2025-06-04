using wumpapi.game.Items;

namespace wumpapi.Services;
/// <summary>
/// Service for storing items
/// </summary>
public interface IItemRegistry
{
    IItem RegisterItem(IItem item);
    IItem? Parse(string itemid);
    List<IItem> GetItems(); 
    void AddToWin(IItem itemid);
    bool Wins(IItem[] itemid);
    bool IsWinItem(IItem itemid);
}