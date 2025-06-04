namespace wumpapi.game.Items.interfaces;

public interface IInProgressItem : IItem
{
    public long StartedAt { get; }
    public int RemainingSeconds { get; }
}