using wumpapi.game.Items.interfaces;

namespace wumpapi.game.events;

public class PlayerStartMakingItemEvent(Player player, IInProgressItem inProgressItem) : Event
{
    public Player Player { get; } = player;
    public IInProgressItem InProgressItem { get; } = inProgressItem;
}