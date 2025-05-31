using wumpapi.game;
using wumpapi.game.Items;
using wumpapi.neo4j;
using wumpapi.Services;

namespace wumpapi.structures;

public class PlayerData
{
    public PlayerData(){}
    private string Username { get; set; }
    private string[] Items { get; set; }

    public Player ToPlayer(IUserRepository userRepository, IItemRegistry itemRegistry)
    {
        Player player =  new Player(userRepository.GetUser(Username).Result);
        IItem[] items = new IItem[Items.Length];
        for (int i = 0; i < Items.Length; i++)
        {
            string itemid = Items[i];
            items[i] = itemRegistry.Parse(itemid);
        }
        return player;
    }
}

