using wumpapi.structures;

namespace wumpapi.game.events;

public class GraphAddConnectionEvent(GraphLink connection) : Event
{
    public GraphLink Connection { get; } = connection;
}