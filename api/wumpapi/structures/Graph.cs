using wumpapi.game;

namespace wumpapi.structures;
/// <summary>
/// This is only used to export a max readable json file
/// </summary>
public class Graph
{
    public GraphNode[] Nodes { get; }
    public GraphLink[] Links { get; }
    
    public Graph(Player[] players)
    {
        Nodes = new GraphNode[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            Nodes[i] = new GraphNode(i,players[i]);
        }
        Links = new GraphLink[0];
    }
}

public record GraphNode(int id, Player player);

public record GraphLink(int source, int target, string name, int strength, long expiresAt);