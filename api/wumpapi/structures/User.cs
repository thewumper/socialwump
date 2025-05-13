namespace wumpapi.structures;

public class User
{
    public User()
    {
        
    }
    public User(string username, string email, string firstName, string lastName)
    {
        Username = username;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    private sealed class UserEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User? x, User? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Username == y.Username && x.Email == y.Email && x.FirstName == y.FirstName && x.LastName == y.LastName;
        }

        public int GetHashCode(User obj)
        {
            return HashCode.Combine(obj.Username, obj.Email, obj.FirstName, obj.LastName);
        }
    }

    public static IEqualityComparer<User> UserComparer { get; } = new UserEqualityComparer();
}