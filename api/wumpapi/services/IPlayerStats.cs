using wumpapi.structures;

namespace wumpapi.Services;

public interface IPlayerStats
{
    PlayerStat GetPlayerStat(User user);
    PlayerStat[] GetLeaderboard(PlayerStatTypes type);
}