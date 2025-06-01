namespace wumpapi.game.Items.interfaces;
/// <summary>
/// Item that modifies the stats aof the owner
/// </summary>
public interface IStatModifyingItem : IItem
{
    StatModifier StatModifier { get;  }
}