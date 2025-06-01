namespace wumpapi.game.Items.interfaces;

public interface ITargetedConsumableItem : ITargetableItem
{
    int RemainingUses { get; }
}