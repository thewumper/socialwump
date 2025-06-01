using MemoryPack;

namespace wumpapi.game.Items;
/// <summary>
/// Item in game
/// </summary>
public partial interface IItem
{
    // All items have sets because the copying thing needs them to have it
    string Name { get; }
    string Id { get; }
    string Description { get; }
    int Price { get;  }
    int BuildTime { get;  }
    string[] Conflicts { get; }
    string[] Requirements { get; }

    bool IsWork(IItem[] items);
}