using wumpapi.game;
using wumpapi.game.events;

namespace wumpapi.structures;
/// <summary>
/// This is only used to export a max readable json file
/// </summary>
public class Graph
{
    public List<GraphNode> Nodes { get; private set; }

    public List<GraphLink> Links => playerConnections.Select(GraphLink).ToList();

    private GraphLink GraphLink(PlayerConnection pc)
    {
        int id1 = -1;
        int id2 = -1;
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].player == pc.P1) id1 = i;
            if (Nodes[i].player == pc.P2) id2 = i;
        }
        return new GraphLink(id1, id2, "Player connection", pc.Strength, pc.ExpiresAt);
    }

    private readonly List<PlayerConnection> playerConnections;
    public Graph(Player[] players)
    {
        Nodes = new();
        playerConnections = new();
        for (int i = 0; i < players.Length; i++)
        {
            Nodes[i] = new GraphNode(i,players[i]);
        }
    }

    public List<IEvent> AddNode(Player player)
    {
        List<IEvent> events = new();
        GraphNode graphNode = new GraphNode(Nodes.Count, player);
        Nodes.Add(graphNode);
        events.Add(new GraphAddNodeEvent(graphNode));
        return events;
    }

    public List<IEvent> UpdateConnection(Player p1, Player p2, int modifier)
    {
        List<IEvent> events = new();
        PlayerConnection? connection = null;
        foreach (var playerConnection in playerConnections)
        {
            if (playerConnection.P1 == p1 && playerConnection.P2 == p2)
            {
                connection = playerConnection;
            }
        }
        PlayerConnection? initially = connection;
        connection ??= new PlayerConnection(p1, p2, 0, modifier);
        connection.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(Constants.ConnectionExpiresSeconds).ToUnixTimeMilliseconds();
        if (Math.Sign(modifier) != Math.Sign(connection.Strength))
        {
            connection.Strength = modifier;
        }
        else
        {
            connection.Strength += modifier;
        }

        if (initially != null)
        {
            events.Add(new RemoveConnectionEvent(GraphLink(initially)));
        }
        else
        {
            playerConnections.Add(connection);
        }
        
        events.Add(new GraphAddConnectionEvent(GraphLink(connection)));
        return events;
    }

    public List<IEvent> TrimOldConnections()
    {
        List<IEvent> events = new();
        foreach (var playerConnection in playerConnections.ToList())
        {
            if (playerConnection.ExpiresAt >= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            {
                playerConnections.Remove(playerConnection);
                events.Add(new RemoveConnectionEvent(GraphLink(playerConnection)));
            }
        }
        return events;
    }
    
}

public record GraphNode(int id, Player player);

public record GraphLink(int Source, int Target, string Name, int Strength, long ExpiresAt);

public class PlayerConnection(Player p1, Player p2, long expiresAt, int strength)
{
    public Player P1 { get; set; } = p1;
    public Player P2 { get; set; } = p2;
    public long ExpiresAt { get; set; } = expiresAt;
    public int Strength { get; set; } = strength;
}