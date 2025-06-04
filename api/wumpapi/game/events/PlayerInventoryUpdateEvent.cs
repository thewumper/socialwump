namespace wumpapi.game.events;

public class PlayerInventoryUpdateEvent(Player player) : Event
{
    public Player Player { get; } = player;
}