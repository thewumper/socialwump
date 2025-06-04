namespace wumpapi.game.events;

public class AllianceCreatedEvent(Alliance alliance) : Event
{
    public Alliance Alliance { get; } = alliance;
}