namespace wumpapi.game.Items.interfaces;
/// <summary>
/// Item that is usable by the owner and has a limited amount of uses
/// </summary>
public interface IConsumableItem : IUsableItem
{
    int RemainingUses { get; }
}