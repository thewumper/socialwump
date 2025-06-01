using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items;

public interface IConsumableItem : IUsableItem
{
    int RemainingUses { get; }
}