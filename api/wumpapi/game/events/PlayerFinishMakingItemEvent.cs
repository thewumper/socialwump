namespace wumpapi.game.events;

public class PlayerFinishMakingItemEvent(Player player, IItem item) : Event
{
    public Player Player { get; } = player;
    public IItem Item { get; } = item;
}