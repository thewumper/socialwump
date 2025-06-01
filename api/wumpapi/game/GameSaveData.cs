using wumpapi.game;

namespace wumpapi.Services;

public class GameSaveData
{
    public List<string> SavedAlliances =  new List<string>();
    public GameState State { get; set; }
}