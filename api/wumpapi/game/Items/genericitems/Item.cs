using MemoryPack;

namespace wumpapi.game.Items.genericitems;

public class Item(string name, string id, string description, int price, string[] conflicts, string[] requirements)
    : IItem
{
    public string Name { get; } = name;
    public string Id { get; } = id;
    public string Description { get; } = description;
    public int Price { get; } = price;
    public string[] conflicts { get; } = conflicts;
    public string[] requirements { get; } = requirements;
    
}