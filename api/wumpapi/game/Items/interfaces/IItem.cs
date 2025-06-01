using wumpapi.game.Items.genericitems;

/// <summary>
/// Item in game
/// </summary>
public interface IItem
{
    // All items have sets because the copying thing needs them to have it
    string Name { get; }
    string Id { get; }
    ItemClassType ClassType { get; }
    string Description { get; }
    int Price { get;  }
    int BuildTime { get;  }
    string[] Conflicts { get; }
    string[] Requirements { get; }
    
    bool IsWork(IItem[] items);
}