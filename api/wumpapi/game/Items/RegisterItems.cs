using wumpapi.game.Items.genericitems;
using wumpapi.Services;

namespace wumpapi.game.Items;

public class ItemRegisterer
{
    public static void RegisterItems(IItemRegistry itemRegistry)
    {
        itemRegistry.RegisterItem(new Item("example", "sg_example", ItemClassType.Generic, "This is an example description", 1,5, [], []));
        
        ///////////////////
        // GENERIC ITEMS //
        ///////////////////
        itemRegistry.RegisterItem(new Weapon("Gun", "ag_gun", ItemClassType.Generic,"Receive a gun to attack others\nDeals 1 Power of Damage, Fires every 30 seconds", 4, 30,[],[],30,
            (activator, target) =>
            {
                // Deal damage to enemy!!!
                target.TakeDamage(activator.GetDamage());
                
                return true;
            }));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Basic Extra Health", "ig_harden", ItemClassType.Generic, "Increases Max Power by 5", 4, 5, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 5) }
            })));

        itemRegistry.RegisterItem(new StatModifyingItem("Basic Increased Power Generation", "ig_powerGenerate", ItemClassType.Generic, "Increases Power Generation by 20%", 4, 10, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 1.2f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Basic Damage Resistance", "ig_damageResistance", ItemClassType.Generic, "Increases Damage Resistance by 10%", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.1f) }
            })));
        
        ///////////////
        // DPS ITEMS //
        ///////////////
        itemRegistry.RegisterItem(new StatModifyingItem("Faster Attacks", "id_tier1", ItemClassType.DPS, "Attacks are 35% faster\nIncrease Power Generation by 10%", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.WeaponCooldownReduction, new Modifier(StatModifierType.Additive, 0.35f) },
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 1.1f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Better Attacks", "id_tier2", ItemClassType.DPS, "Increase Attack Damage by 2\nIncrease Max Power by 5", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.AttackPower, new Modifier(StatModifierType.Additive, 2) },
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 5) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Way Faster Attacks", "id_tier3", ItemClassType.DPS, "Attacks are 50% faster\nIncrease damage resistance by 10%", 16, 60, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.WeaponCooldownReduction, new Modifier(StatModifierType.Additive, 0.5f) },
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.1f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Way Better Attacks", "id_tier4", ItemClassType.DPS, "Increase attack damage by 6\nIncrease damage resistance by 10%", 32, 120, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.AttackPower, new Modifier(StatModifierType.Additive, 6) },
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.1f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Life Steal", "sd_tier5", ItemClassType.DPS, "Heal for 20% of all damage dealt", 32, 120, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.LifeSteal, new Modifier(StatModifierType.Additive, 0.2f) },
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Insane Life Steal", "sd_tier6", ItemClassType.DPS, "Heal for all damage dealt\nAttacks are 75% faster", 100, 600, [], ["sd_tier5"],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.LifeSteal, new Modifier(StatModifierType.Additive, 1.0f) },
                { StatType.WeaponCooldownReduction, new Modifier(StatModifierType.Additive, 0.75f)}
            })));
        
        
    }
}