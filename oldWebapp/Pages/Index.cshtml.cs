using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using socialweb.Models;


namespace socialweb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private Person person = new Person()
    {
        Id = Guid.NewGuid(),
        FirstName = "Atticus",
        LastName = "Pak",
        MoviesAsActor = new List<Movie>()
        {
            new Movie()
            {
                Title = "Iron Man 5"
            }
        }
    };

    applicationGraphContext

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}