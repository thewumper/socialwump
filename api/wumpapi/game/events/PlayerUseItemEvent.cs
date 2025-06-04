namespace wumpapi.game.events;

public class PlayerUseItemEvent(Player player, Player? target, IItem item, bool isPositive) : Event
{
    public Player Player { get; } = player;
    public Player? Target { get; } = target;
    public IItem Item { get; } = item;
    public bool IsPositive { get; } = isPositive;
}