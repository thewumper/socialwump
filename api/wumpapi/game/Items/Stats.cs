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
            List<StatModifier> additiveBuffs = new List<StatModifier>();
            List<StatModifier> multiplicativeBuffs = new List<StatModifier>();
            if (item is IStatModifyingItem modifyingItem)
            {
                if (modifyingItem.StatModifier.ModifierType == StatModifierType.Additive)
                {
                    additiveBuffs.Add(modifyingItem.StatModifier);
                }
                else if (modifyingItem.StatModifier.ModifierType == StatModifierType.Multiplicative)
                {
                    multiplicativeBuffs.Add(modifyingItem.StatModifier);
                }
            }
            Dictionary<StatType, float> newStats = BaseStats.ToDictionary(stat => stat.Key, stat => stat.Value);
            foreach (StatModifier additiveBuff in additiveBuffs)
            {
                foreach (var modifier in additiveBuff.Modifiers)
                {
                    newStats[modifier.Key] += modifier.Value;
                }
            }

            foreach (StatModifier multiplicativeBuff in multiplicativeBuffs)
            {
                foreach (var modifier in multiplicativeBuff.Modifiers)
                {
                    newStats[modifier.Key] *= modifier.Value;
                }
            }
            // Kind of hacky, just get it working though
            newStats[StatType.Power] = CurrentStats[StatType.Power];
            
            CurrentStats = new Dictionary<StatType, float>(newStats);
        }
    }
}
public class StatModifier(StatModifierType modifierType, Dictionary<StatType, float> modifiers)
{
    public StatModifierType ModifierType { get; } = modifierType;
    public Dictionary<StatType, float> Modifiers { get; } = modifiers;
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