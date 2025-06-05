using wumpapi.structures;

namespace wumpapi.game.events;

public class GraphAddConnectionAttempt(GraphLink connection) : Event
{
    public GraphLink Connection { get; } = connection;
}