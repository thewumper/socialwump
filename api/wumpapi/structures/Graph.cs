namespace wumpapi.structures;

public class Graph
{
    public GraphNode[] Nodes { get; }
    public GraphLink[] Links { get; }

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

public record GraphNode(int id, User user);
public record GraphLink(int source, int target, string name, string data);