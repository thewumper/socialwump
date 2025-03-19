namespace socialweb.Models;

public class Movie
{
    public Guid Id { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public List<Person> Actors { get; private set; } = [];
    public List<Person> Drectors { get; private set; } = [];
}