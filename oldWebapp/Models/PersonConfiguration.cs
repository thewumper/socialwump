using Neo4j.Berries.OGM.Enums;
using Neo4j.Berries.OGM.Models.Config;

namespace socialweb.Models;

public class PersonConfiguration
{
     public void Configure(NodeTypeBuilder<Person> builder)
      {
        builder.HasRelationWithMultiple(x => x.MoviesAsActor, "ACTS_IN", RelationDirection.Out);
        builder.HasRelationWithMultiple(x => x.MoviesAsDirector, "DIRECTS", RelationDirection.Out);
      }
}