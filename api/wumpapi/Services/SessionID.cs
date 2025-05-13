namespace wumpapi.Services;

public class SessionID
{
    public static readonly TimeSpan duration = TimeSpan.FromMinutes(30);
    public string SSID { get; private set; }

    public bool Expired => DateTime.UtcNow > expires;

    private DateTime expires;

    public void Refresh()
    {
        expires = DateTime.UtcNow.Add(duration);
    }
    public SessionID()
    {
        SSID = Guid.NewGuid().ToString();
        expires = DateTime.UtcNow.Add(duration);
    }
}