using wumpapi.game.Items.interfaces;

namespace wumpapi.game.Items;
/// <summary>
/// Player statistics, allows modifications from items
/// </summary>
public class Stats
{
    public Stats()
    {
        CurrentStats = new Dictionary<StatType, float>();
        foreach (var stat in BaseStats)
        {
            CurrentStats.Add(stat.Key, stat.Value);
        }
    }

    private static readonly Dictionary<StatType, float> BaseStats = new()
    {
        { StatType.MaxPower, 5 },
        { StatType.PowerGenerationAmount, 1 },
        { StatType.DamageResistance, 0.1f },
        { StatType.DamageShare, 0 },
        { StatType.LifeSteal, 0 },
        { StatType.AttackPower, 1 },
        { StatType.CooldownReduction, 0 },
        { StatType.WeaponCooldownReduction, 0 },
        { StatType.PowerGenerationPeriod, 15 }
    };
    public int Power { get; set; }
    public Dictionary<StatType, float> CurrentStats { get; private set; }
    

    public void UpdateFromItems(IItem?[] items)
    {
        foreach (IItem? item in items)
        {
            if (item == null)
            {
                continue;
            }
            List<StatModifier> buffs = new();
            if (item is IStatModifyingItem modifyingItem)
            {
                buffs.Add(modifyingItem.StatModifier);
            }
            var newStats = BaseStats.ToDictionary(stat => stat.Key, stat => stat.Value);
            var newStatsMultiply = BaseStats.ToDictionary(stat => stat.Key, _ => 1f);

            foreach (StatModifier buff in buffs)
            {
                foreach (var modifier in buff.Modifiers)
                {
                    switch (modifier.Value.ModiferType)
                    {
                        case StatModifierType.Additive:
                            newStats[modifier.Key] += modifier.Value.Amount;
                            break;
                        case StatModifierType.Multiplicative:
                            newStatsMultiply[modifier.Key] += modifier.Value.Amount;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            foreach (var stat in newStats)
            {
                newStats[stat.Key] = stat.Value * newStatsMultiply[stat.Key];
            }
            
            CurrentStats = new Dictionary<StatType, float>(newStats);
        }
    }
}
public class StatModifier(Dictionary<StatType, Modifier> modifiers)
{
    public Dictionary<StatType, Modifier> Modifiers { get; } = modifiers;
}

public class Modifier(StatModifierType modiferType, float amount)
{
    public StatModifierType ModiferType { get; } = modiferType;
    public float Amount { get; } = amount;
}
public enum StatModifierType
{
    Additive,
    Multiplicative,
}

public enum StatType
{
    MaxPower,
    PowerGenerationAmount,
    DamageResistance,
    DamageShare,
    LifeSteal,
    AttackPower,
    PowerGenerationPeriod,
    CooldownReduction,
    WeaponCooldownReduction
}