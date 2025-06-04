namespace wumpapi.game.events;

public class PlayerSharePowerEvent(Player player, Player recipient, int power) : Event
{
    public Player Player { get; } = player;
    public Player Recipient { get; } = recipient;
    public int Power { get; } = power;
}