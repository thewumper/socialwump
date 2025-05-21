namespace wumpapi.configuration;
/// <summary>
/// For loading the neo4j configuration settings from a file because C# has schenigans
/// </summary>
public class ApplicationSettings
{
    
    public Uri Neo4jConnection { get; set; }

    public string Neo4jUser { get; set; }

    public string Neo4jPassword { get; set; }

    public string Neo4jDatabase { get; set; }
}