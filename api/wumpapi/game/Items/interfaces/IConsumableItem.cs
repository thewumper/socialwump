namespace wumpapi.game.Items;

public interface IConsumableItem : IUsableItem
{
    int RemainingUses { get; }
}