namespace wumpapi.game.Items;

public interface IStatModifyingItem : IItem
{
    StatModifier StatModifier { get;  }
}