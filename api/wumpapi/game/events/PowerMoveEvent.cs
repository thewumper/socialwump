namespace wumpapi.game.events;

public class PowerMoveEvent(Player initiator, Player recipient, int amount) : Event
{
    public Player Initiator { get; } = initiator;
    public Player Recipient { get; } = recipient;
    public int Amount { get; } = amount;
}