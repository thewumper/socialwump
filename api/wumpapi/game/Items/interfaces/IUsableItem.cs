namespace wumpapi.game.Items;

public interface IUsableItem : IItem
{
    float Cooldown { get; }
    bool Use(Player activator);
}