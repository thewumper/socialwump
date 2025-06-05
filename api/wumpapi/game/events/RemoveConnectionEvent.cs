using wumpapi.structures;

namespace wumpapi.game.events;

public class RemoveConnectionEvent(GraphLink removed) : Event
{
    public GraphLink Removed { get; } = removed;
}