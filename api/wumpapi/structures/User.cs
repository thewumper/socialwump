using wumpapi.neo4j;

namespace wumpapi.structures;

public class User
{
    public User()
    {
        
    }
    public User(string username, string email, string firstname, string lastname)
    {
        Username = username;
        Email = email;
        Firstname = firstname;
        Lastname = lastname;
    }

    public string Username { get; set; }
    public string? Password { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }

    protected bool Equals(User other)
    {
        return Username == other.Username;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((User)obj);
    }

    public override int GetHashCode()
    {
        return Username.GetHashCode();
    }

    public override string ToString()
    {
        return Username;
    }

    public void Censor()
    {
        Password = null;
    }
}