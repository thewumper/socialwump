using wumpapi.game.Items.genericitems;
using wumpapi.Services;

namespace wumpapi.game.Items;

public class ItemRegisterer
{
    public static void RegisterItems(IItemRegistry itemRegistry)
    {
        itemRegistry.RegisterItem(new Item("example", "sG_example", ItemClassType.Generic, "This is an example description", 1,5, [], []));
        
        ///////////////////
        // GENERIC ITEMS //
        ///////////////////
        itemRegistry.RegisterItem(new TargetableItem("Gun", "aG_gun", ItemClassType.Generic,"Receive a gun to attack others\nDeals 1p, Fires every 30 seconds", 4, 30,[],[],30,
            (activator, target) =>
            {
                // Deal damage to enemy!!!
                target.TakeDamage(activator.GetDamage());
                
                return true;
            }));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Basic Extra Health", "iG_harden", ItemClassType.Generic, "Increases Max Power by 5", 4, 5, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 5) }
            })));

        itemRegistry.RegisterItem(new StatModifyingItem("Basic Increased Power Generation", "iG_powerGenerate", ItemClassType.Generic, "Increases Power Generation by 20%", 4, 10, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 1.2f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Basic Damage Resistance", "iG_damageResistance", ItemClassType.Generic, "Increases Damage Resistance by 10%", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.1f) }
            })));
        
        ///////////////
        // DPS ITEMS //
        ///////////////
        itemRegistry.RegisterItem(new StatModifyingItem("Faster Attacks", "iD_tier1", ItemClassType.DPS, "Attacks are 35% faster\nIncrease Power Generation by 10%", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.CooldownReduction, new Modifier(StatModifierType.Multiplicative, 0.35f) },
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 1.1f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Better Attacks", "iD_tier2", ItemClassType.DPS, "Increase Attack Damage by 2\nIncrease Max Power by 5", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.AttackPower, new Modifier(StatModifierType.Additive, 2) },
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 5) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Way Faster Attacks", "iD_tier3", ItemClassType.DPS, "Attacks are 50% faster\nIncrease damage resistance by 10%", 16, 60, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.CooldownReduction, new Modifier(StatModifierType.Additive, 0.5f) },
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.1f) }
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Way Better Attacks", "iD_tier4", ItemClassType.DPS, "Increase attack damage by 6\nIncrease damage resistance by 10%", 32, 120, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.AttackPower, new Modifier(StatModifierType.Additive, 6) },
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.1f) }
            })));
    }
}