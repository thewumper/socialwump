namespace wumpapi.game.events;

public class PlayerJoinAllianceEvent(Player player, Alliance alliance) : Event
{
    public Player Player { get; } = player;
    public Alliance Alliance { get; } = alliance;
}