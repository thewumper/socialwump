using Neo4j.Berries.OGM.Enums;
using Neo4j.Berries.OGM.Models.Config;

namespace socialweb.Models;

public class MovieConfiguration
{
 public void Configure(NodeTypeBuilder<Movie> builder)
 {
  builder.HasRelationWithMultiple(x => x.Actors, "ACTS_IN", RelationDirection.In);
  builder.HasRelationWithMultiple(x => x.Directors, "DIRECTS", RelationDirection.In);
 }
}