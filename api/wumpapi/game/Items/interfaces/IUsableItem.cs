namespace wumpapi.game.Items.interfaces;

public interface IUsableItem : ICooldownItem
{
    bool Use(Player activator);
}