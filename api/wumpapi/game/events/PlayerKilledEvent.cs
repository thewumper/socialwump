namespace wumpapi.game.events;

public class PlayerKilledEvent(Player? killer, Player player) : Event
{
    public Player Player { get; } = player;
    public Player? Killer { get; } = killer;
}