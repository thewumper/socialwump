using wumpapi.neo4j;
using wumpapi.Services;

namespace wumpapi.game;
/// <summary>
/// Data that will be pulled to and from neo4j, can be converted into a player
/// </summary>
public class PlayerData
{
    public PlayerData(){}

    public PlayerData(string username, string[] items, string? alliance)
    {
        Username = username;
        Items = items;
        Alliance = alliance;
    }

    public string Username { get; set; }
    public string[] Items { get; set; }
    public string? Alliance { get; set; }
    
    
    public Player ToPlayer(Game game, IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        Player player = new Player(userRepository.GetUser(Username).Result, game);
        IItem?[] items = new IItem[Items.Length];
        for (int i = 0; i < Items.Length; i++)
        {
            string itemid = Items[i];
            items[i] = itemRegistry.Parse(itemid);
        }
        player.Items = items;
        player.AllianceName = Alliance;
        return player;
    }
}

