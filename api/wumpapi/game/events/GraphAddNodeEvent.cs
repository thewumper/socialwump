using wumpapi.structures;

namespace wumpapi.game.events;

public class GraphAddNodeEvent(GraphNode node) : Event
{
    public GraphNode Node { get; } = node;
}