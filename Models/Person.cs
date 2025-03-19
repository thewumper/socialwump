namespace socialweb.Models;

public class Person
 {
   public Guid Id { get; set; }
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public List<Movie> MoviesAsActor { get; private set; } = [];
   public List<Movie> MoviesAsDirector { get; private set; } = [];
 }