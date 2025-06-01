namespace wumpapi.game.Items.interfaces;

public interface ITargetableItem : ICooldownItem
{
    bool Use(Player activator, Player target);
}