using wumpapi.structures;

namespace wumpapi.Services;

public class PlayerStats : IPlayerStats
{
    private readonly Dictionary<User, PlayerStat> playerStats = new();
    public PlayerStat GetPlayerStat(User user)
    {
        if (playerStats.TryGetValue(user, out var playerStat))
        {
            return playerStat;
        }

        PlayerStat stat = new PlayerStat(user);
        playerStats.Add(user, stat);
        return stat;
    }

    public PlayerStat[] GetLeaderboard(PlayerStatTypes type)
    {
        List<PlayerStat> stats = playerStats.Values.ToList();
        stats.Sort((s1, s2) => s1.GetStat(type) > s2.GetStat(type) ? 1 : -1);
        return stats.ToArray();
    }
}

public class PlayerStat(User user)
{
    private readonly Dictionary<PlayerStatTypes, int> stats = new();
    private readonly User user = user;

    public void SetStat(PlayerStatTypes type, int value)
    {
        stats[type] = value;
    }

    public int GetStat(PlayerStatTypes type)
    {
        return stats[type];
    }

    public User GetUser()
    {
        return user;
    }
    
}

public enum PlayerStatTypes
{
    Wins,
    Losses,
    Damage,
    DamageTaken,
    Deaths,
    Kills,
    Revives,
    RevivesTaken,
    Healing,
    ItemsBuilt,
    ItemsUsed,
}