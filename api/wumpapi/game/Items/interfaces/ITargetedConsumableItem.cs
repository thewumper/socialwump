namespace wumpapi.game.Items.interfaces;
/// <summary>
/// ITem that has limited uses and can be used to target a player
/// </summary>
public interface ITargetedConsumableItem : ITargetableItem
{
    int RemainingUses { get; }
}