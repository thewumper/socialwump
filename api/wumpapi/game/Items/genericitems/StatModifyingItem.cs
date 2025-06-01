
namespace wumpapi.game.Items.genericitems;
public class StatModifyingItem(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] conflicts, string[] requirements,  StatModifier statModifier)
    : Item(name, id, classType, description, price, buildTime, conflicts, requirements), IStatModifyingItem
{
    public StatModifier StatModifier { get; } = statModifier;
}