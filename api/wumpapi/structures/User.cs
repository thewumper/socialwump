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

    private sealed class UserEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User? x, User? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Username == y.Username && x.Email == y.Email && x.Firstname == y.Firstname && x.Lastname == y.Lastname;
        }

        public int GetHashCode(User obj)
        {
            return HashCode.Combine(obj.Username, obj.Email, obj.Firstname, obj.Lastname);
        }
    }

    public static IEqualityComparer<User> UserComparer { get; } = new UserEqualityComparer();

    public void Censor()
    {
        Password = null;
    }
}