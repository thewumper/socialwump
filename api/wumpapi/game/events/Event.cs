namespace wumpapi.game.events;

public abstract class Event : IEvent
{
    public string Name => GetType().Name;
    public long InitiatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}