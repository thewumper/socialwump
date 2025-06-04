namespace wumpapi.game.events;

public interface IEvent
{

    public string Name { get; }
    public long InitiatedAt { get; }
}