using wumpapi.Services;

namespace wumpapi.game;

public class GameSaveData
{
    public List<string> SavedAlliances { get; set; }
    public GameState State { get; set; }

    public GameSaveData(GameState state, List<string> savedAlliances)
    {
        State = state;
        SavedAlliances = savedAlliances;
    }

    public GameSaveData()
    {
        SavedAlliances = new List<string>();
    }
}