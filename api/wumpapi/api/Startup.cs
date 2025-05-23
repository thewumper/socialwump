namespace wumpapi.api;

public class Startup
{
    public static void Main(string[] args)
    {
        Webapp webapp = new Webapp(args);
        webapp.Start();
    }
}