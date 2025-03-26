using Neo4j.Berries.OGM.Contexts;
using Neo4j.Berries.OGM.Models.Sets;

namespace socialweb.Models;

public class ApplicationGraphContext(Neo4jOptions options) : GraphContext(options)
{
    public NodeSet<Person> People { get; private set; }
    public NodeSet<Movie> Movie { get; private set; }
}