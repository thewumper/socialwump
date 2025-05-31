namespace wumpapi.game.Items;

public interface ITargetedConsumableItem : ITargetableItem
{
    int RemainingUses { get; }
}