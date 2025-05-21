namespace wumpapi.structures;
/// <summary>
/// This is only used to export a max readable json file
/// </summary>
public class Graph
{
    public GraphNode[] Nodes { get; }
    public GraphLink[] Links { get; }
    
    // TODO: Fix this it's awful
    /// <summary>
    /// Create a max readable graph
    /// </summary>
    /// <param name="users">Users in the graph</param>
    /// <param name="connections">Connections between users</param>
    /// <exception cref="ArgumentException">If connections reference users that are not provided</exception>
    public Graph(User[] users,Connection[] connections)
    {
        Nodes = new GraphNode[users.Length];
        for (var i = 0; i < users.Length; i++)
        {
            var user = users[i];
            Nodes[i] = new GraphNode(i, user);
        }

        Links = new GraphLink[connections.Length];
        for (var i = 0; i < connections.Length; i++)
        {
            var connection = connections[i];
            int source = -1, target = -1;
            // TODO: Not very good performance here prob, better way to do this 1000%
            foreach (GraphNode node in Nodes)
            {
                if (Equals(node.user, connection.Initiator))
                    source = node.id;
                else if (Equals(node.user, connection.Recipient))
                    target = node.id;
                if (source != -1 && target != -1)
                {
                    break;
                }
            }

            if (target == -1 || source == -1)
            {
                throw new ArgumentException("Connection contains users that are not provided");
            }

            Links[i] = new GraphLink(source, target, connection.Name, connection.Data);
        }
    }
}
/// <summary>
/// Node on a max readable grpah
/// </summary>
/// <param name="id">numerical id</param>
/// <param name="user">refrence to the user it repersents</param>
public record GraphNode(int id, User user);
/// <summary>
/// Link on a max readable grpah
/// </summary>
/// <param name="source">id of source node</param>
/// <param name="target">id of target node</param>
/// <param name="name">This connections name</param>
/// <param name="data">Data associated with this connection</param>
public record GraphLink(int source, int target, string name, string data);