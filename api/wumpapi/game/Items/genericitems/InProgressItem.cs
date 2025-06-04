using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;

public class InProgressItem : IInProgressItem
{
    private readonly IItem inProgress;

    public InProgressItem(Player player, IItem item)
    {
        inProgress = item;
        Name = $"In progress: {item.Name}";
        Id = $"inp_{item.Id}";
        ClassType = item.ClassType;
        Description = $"When completed: {item.Description}";
        BuildTime = item.BuildTime;
        StartedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public string Name { get; }
    public string Id { get; } // conform to jaydens silly naming convention :D
    public ItemClassType ClassType { get; }
    public string Description { get; }
    public int Price => 0;
    public int BuildTime { get; }
    public string[] Conflicts => inProgress.Conflicts;
    public string[] Requirements => inProgress.Requirements;
    public long StartedAt { get; }
    public int RemainingSeconds => (DateTimeOffset.FromUnixTimeMilliseconds(StartedAt) + TimeSpan.FromSeconds(BuildTime) - DateTimeOffset.UtcNow).Seconds;
    public bool IsWork(IItem[] items)
    {
        return false;
    }
}