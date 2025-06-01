using wumpapi.structures;

namespace wumpapi.Services;
/// <summary>
/// User statistics tracker + leaderboard
/// </summary>
public interface IPlayerStats
{
    PlayerStat GetPlayerStat(User user);
    PlayerStat[] GetLeaderboard(PlayerStatTypes type);
}