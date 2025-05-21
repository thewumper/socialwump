using wumpapi.neo4j;

namespace wumpapi.structures;
/// <summary>
/// Simple class representing a user,
/// Make sure you call <see cref="Censor"/> if you plan on exporting it
/// </summary>
public class User
{
    /// <summary>
    /// Create a user with no data, (for conversion from dictionaries),
    /// maybe try passing things to constructors instead in the future for conversions?
    /// </summary>
    public User()
    {
        
    }
    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="username">Username, how users are identified</param>
    /// <param name="email">Email</param>
    /// <param name="firstname">First name</param>
    /// <param name="lastname">Last name</param>
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
    
    /// <summary>
    /// Removes any reference to the password
    /// </summary>
    public void Censor()
    {
        Password = null;
    }
}