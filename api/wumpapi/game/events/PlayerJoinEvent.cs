namespace wumpapi.game.events;

public class PlayerJoinEvent(Player player) : Event
{
    public Player Player { get; } = player;
}