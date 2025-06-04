namespace wumpapi.game.events;

public class AllianceWinEvent(Alliance alliance) : Event
{
    public Alliance Alliance { get; } = alliance;
}