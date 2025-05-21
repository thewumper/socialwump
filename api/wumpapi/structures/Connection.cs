namespace wumpapi.structures;

public class Connection
{
    public User Initiator { get; set; }
    public User Recipient { get; set; }
    public String Name { get; set; }
    public String Data { get; set; }

    public Connection() {}
    public Connection(User initiator, User recipient, String name, String data)
    {
        Initiator = initiator;
        Recipient = recipient;
        Name = name;
        Data = data;
    }
}