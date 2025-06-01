using wumpapi.Services;

namespace wumpapi.game;
/// <summary>
/// Data that will be pulled to and from neo4j
/// </summary>
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