using wumpapi.game.Items.genericitems;
using wumpapi.game.Items.interfaces;
using wumpapi.Services;

namespace wumpapi.game.Items;
/// <summary>
/// Registers all items in the game
/// </summary>
public class ItemRegisterer
{
    public static void RegisterItems(IItemRegistry itemRegistry)
    {
        itemRegistry.RegisterItem(new Item("example", "sg_example", ItemClassType.Generic, "This is an example description", 1,5, [], []));
        
        ///////////////////
        // GENERIC ITEMS //
        ///////////////////
        itemRegistry.RegisterItem(new Weapon("Gun", "ag_gun", ItemClassType.Generic,"Receive a gun to attack others\nDeals 1 Power of Damage, 30 Second Cooldown", 4, 30,[],[],30,
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
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.8f) }
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
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.9f) }
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
        
        ///////////////////
        // SUPPORT ITEMS //
        ///////////////////
        itemRegistry.RegisterItem(new StatModifyingItem("Faster Power Generation", "is_tier1", ItemClassType.Support, "Increase power generation by 50%\nAttacks are 15% slower", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.5f) },
                { StatType.WeaponCooldownReduction, new Modifier(StatModifierType.Additive, -0.15f)}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Even Faster Power Generation", "is_tier2", ItemClassType.Support, "Increase power generation by 50%\nLower damage resistance by 10%", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.5f) },
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, -0.1f)}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Way Faster Power Generation", "is_tier3", ItemClassType.Support, "Increase power generation by 50%\nIncrease Max Power by 10", 16, 60, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.5f) },
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 10f)}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Way Even Faster Power Generation", "is_tier4", ItemClassType.Support, "Increase power generation by 50%\nIncrease Max Power by 20", 32, 120, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.5f) },
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 10f)}
            })));
        
        // This item does not have full functionality
        itemRegistry.RegisterItem(new TargetableItem("Power Share", "ss_tier5", ItemClassType.Support, "Choose player in alliance, for the next two minutes your power generation items are shared", 32, 120, [], [], 180,
            (activator, target) =>
            {
                return true;
            }));
        
        ////////////////
        // TANK ITEMS //
        ////////////////
        itemRegistry.RegisterItem(new StatModifyingItem("Increased Power", "it_tier1", ItemClassType.Tank, "Increase Max Power by 15\nLower power generation by 20%", 8, 30, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 15f) },
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.8f)}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Increased Resistance", "it_tier2", ItemClassType.Tank, "Increase damage resistance by 20%\nLower power generation by 10%", 16, 60, [], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.2f) },
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.9f)}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("More Power", "it_tier3", ItemClassType.Tank, "Increase Max Power by 30\nLower power generation by 10%\nConflicts with all support items", 16, 60, ["is_tier1","is_tier2","is_tier3","is_tier4","ss_tier5"], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 30f) },
                { StatType.PowerGenerationPeriod, new Modifier(StatModifierType.Multiplicative, 0.9f)}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("More Power and Resistance", "it_tier4", ItemClassType.Tank, "Increase Max Power by 50\nIncrease damage resistance by 40%\nConflicts with all support items", 32, 120, ["is_tier1","is_tier2","is_tier3","is_tier4","ss_tier5"], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.MaxPower, new Modifier(StatModifierType.Additive, 50f) },
                { StatType.DamageResistance, new Modifier(StatModifierType.Additive, 0.4f )}
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Absorb Some Damage", "st_tier5", ItemClassType.Tank, "Absorb 20% of damage dealt to other alliance members\nConflicts with all support items", 32, 300, ["is_tier1","is_tier2","is_tier3","is_tier4","ss_tier5"], [],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.DamageShare, new Modifier(StatModifierType.Additive, 0.2f) },
            })));
        
        itemRegistry.RegisterItem(new StatModifyingItem("Absorb All Damage", "st_tier6", ItemClassType.Tank, "Absorb all damage dealt to other alliance members\nConflicts with all support items", 64, 600, ["is_tier1","is_tier2","is_tier3","is_tier4","ss_tier5"], ["st_tier5"],
            new StatModifier(new Dictionary<StatType, Modifier>
            {
                { StatType.DamageShare, new Modifier(StatModifierType.Additive, 1.0f) },
            })));
        
        ////////////////
        // MACGUFFINS //
        ////////////////
        itemRegistry.RegisterItem(new Item("Red MacGuffin", "sm_red", ItemClassType.MacGuffin, "Can only hold one per person, if all three MacGuffins are held by the same alliance, that alliance is the winner!", 100, 300, ["sm_blue", "sm_green"], []));
        
        itemRegistry.RegisterItem(new Item("Blue MacGuffin", "sm_blue", ItemClassType.MacGuffin, "Can only hold one per person, if all three MacGuffins are held by the same alliance, that alliance is the winner!", 100, 300, ["sm_red", "sm_green"], []));
        
        itemRegistry.RegisterItem(new Item("Green MacGuffin", "sm_green", ItemClassType.MacGuffin, "Can only hold one per person, if all three MacGuffins are held by the same alliance, that alliance is the winner!", 100, 300, ["sm_red", "sm_blue"], []));
        
    }
}