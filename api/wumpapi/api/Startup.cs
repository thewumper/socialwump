namespace wumpapi.api;
/// <summary>
/// Entrypoint for the program. If a method does not have a comment, it is because it is late, and there wasn't a comment there before
/// </summary>
public class Startup
{
    public static void Main(string[] args)
    {
        Webapp webapp = new Webapp(args);
        webapp.Start();
    }
}