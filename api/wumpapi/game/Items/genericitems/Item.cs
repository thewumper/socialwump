
namespace wumpapi.game.Items.genericitems;

public class Item(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] conflicts, string[] requirements)
    : IItem
{
    public string Name { get; } = name;
    public string Id { get; } = id;
    public ItemClassType ClassType { get; } = classType;
    public string Description { get; } = description;
    public int Price { get; } = price;
    public int BuildTime { get; } = buildTime;
    public string[] Conflicts { get; } = conflicts;
    public string[] Requirements { get; } = requirements;

    public bool IsWork(IItem[] items)
    {
        int counter = 0;
        foreach (IItem item in items)
        {
            if (Conflicts.Contains(item.Id))
            {
                return false;
            }
            if (Requirements.Contains(item.Id))
            {
                counter++;
            }
        }

        return counter >= Requirements.Length;
    }
}