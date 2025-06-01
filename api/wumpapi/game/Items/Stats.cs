namespace wumpapi.game.Items;

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
    
    public static readonly Dictionary<StatType, float> BaseStats = new()
    {
        { StatType.Power, 0},
        { StatType.MaxPower, 5 },
        { StatType.PowerGenerationAmount, 1 },
        { StatType.DamageResistance, 0 },
        { StatType.DamageShare, 0},
        { StatType.AttackPower, 1},
        { StatType.CooldownReduction, 0},
        { StatType.PowerGenerationPeriod, 10}
    };
    public Dictionary<StatType, float> CurrentStats { get; private set; }
    

    public void UpdateFromItems(IItem[] items)
    {
        foreach (IItem item in items)
        {
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
            
            // Kind of hacky, just get it working though
            newStats[StatType.Power] = CurrentStats[StatType.Power];
            
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
    Power,
    PowerGenerationAmount,
    DamageResistance,
    DamageShare,
    AttackPower,
    PowerGenerationPeriod,
    CooldownReduction,
}