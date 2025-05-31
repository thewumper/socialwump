namespace wumpapi.game.Items;

public interface ITargetableItem : IItem
{
    float Cooldown { get; }
    bool Use(Player activator, Player target);
}