namespace wumpapi.Services;
/// <summary>
/// Session id for users, mostly used internally (maybe this shouldnt be public)
/// TODO: Figure out if this should be public
/// </summary>
public class SessionID
{
    public static readonly TimeSpan Duration = TimeSpan.FromMinutes(30);
    public string SSID { get; private set; }

    public bool Expired => DateTime.UtcNow > expires;

    private DateTime expires;

    public void Refresh()
    {
        expires = DateTime.UtcNow.Add(Duration);
    }
    public SessionID()
    {
        SSID = Guid.NewGuid().ToString();
        expires = DateTime.UtcNow.Add(Duration);
    }
}