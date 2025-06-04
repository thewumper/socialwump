namespace wumpapi.game.events;

public class PlayerShareItemEvent(Player player, Player recipient, IItem item) : Event
{
    public Player Player { get; } = player;
    public Player Recipient { get; } = recipient;
    public IItem Item { get; } = item;
}