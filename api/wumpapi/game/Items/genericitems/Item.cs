using MemoryPack;

namespace wumpapi.game.Items.genericitems;

public class Item(string name, string id, string description, int price, int buildTime, string[] conflicts, string[] requirements)
    : IItem
{
    public string Name { get; } = name;
    public string Id { get; } = id;
    public string Description { get; } = description;
    public int Price { get; } = price;
    public int BuildTime { get; } = buildTime;
    public string[] conflicts { get; } = conflicts;
    public string[] requirements { get; } = requirements;

    public bool IsWork(IItem[] items)
    {
        int counter = 0;
        foreach (IItem item in items)
        {
            if (conflicts.Contains(item.Id))
            {
                return false;
            }
            if (requirements.Contains(item.Id))
            {
                counter++;
            }
        }

        return counter >= requirements.Length;
    }
}