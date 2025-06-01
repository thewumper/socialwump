namespace wumpapi.game.Items.interfaces;
/// <summary>
/// Item that can be used by the owner
/// </summary>
public interface IUsableItem : ICooldownItem
{
    bool Use(Player activator);
}