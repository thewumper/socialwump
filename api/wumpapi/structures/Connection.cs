namespace wumpapi.structures;
/// <summary>
/// Internally represents a connection on the graph
/// </summary>
public class Connection
{
    public User Initiator { get; set; }
    public User Recipient { get; set; }
    public String Name { get; set; }
    public String Data { get; set; }
    /// <summary>
    /// For dictionary conversion
    /// </summary>
    public Connection() {}
    /// <summary>
    /// Create a connection
    /// </summary>
    /// <param name="initiator">Source of the link</param>
    /// <param name="recipient">Target of the link</param>
    /// <param name="name">Name of the link (might replace this with an enum in the future)</param>
    /// <param name="data">Extra information on this connection, might replace this with some object in the future</param>
    public Connection(User initiator, User recipient, String name, String data)
    {
        Initiator = initiator;
        Recipient = recipient;
        Name = name;
        Data = data;
    }
}