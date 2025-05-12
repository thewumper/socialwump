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
}