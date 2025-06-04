namespace wumpapi.game.events;

public class PlayerUpdatePowerEvent(Player player, int power) : Event
{
    public Player Player { get; } = player;
    public int Power { get; } = power;
}