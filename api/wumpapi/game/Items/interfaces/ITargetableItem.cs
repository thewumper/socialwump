namespace wumpapi.game.Items.interfaces;
/// <summary>
/// Item that is usable on annother player
/// </summary>
public interface ITargetableItem : ICooldownItem
{
    bool Use(Player activator, Player target);
}