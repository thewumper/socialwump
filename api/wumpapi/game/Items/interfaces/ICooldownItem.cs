namespace wumpapi.game.Items.interfaces;
/// <summary>
/// Item that has a cooldown
/// </summary>
public interface ICooldownItem : IItem
{
    public float Cooldown(Player player);
    public float BaseCooldown { get;  }
    public DateTime LastUsed { get; }
    public bool IsUsable(Player player);
    public bool Use(Player player);

}