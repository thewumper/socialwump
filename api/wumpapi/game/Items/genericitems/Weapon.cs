using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items.genericitems;
public class Weapon(string name, string id, ItemClassType classType, string description, int price, int buildTime, string[] requirements, string[] conflicts, float cooldown, Weapon.UseDelegate onUse)
    : TargetableItem(name, id, classType, description, price, buildTime, requirements, conflicts, cooldown, onUse)
{
    private UseDelegate onUse = onUse;
    private float baseWeaponCooldown = cooldown;
    public new bool Use(Player activator, Player target)
    {
        ApplyWeaponCooldowns(activator);
        if (!IsUsable(activator)) return false;
        if (onUse.Invoke(activator, target))
        {
            base.Use(activator);
        }
        return false;
    }

    private void ApplyWeaponCooldowns(Player player)
    {
        BaseCooldown *= 1 - player.Stats.CurrentStats[StatType.WeaponCooldownReduction];
    }
}