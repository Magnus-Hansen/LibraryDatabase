public class BoardgameDetailsMongo
{
    public string No_Of_Players { get; set; }
    public string Play_Time { get; set; }
    public string Age_Group { get; set; } // in minutes

    public int item_id { get; set; }

    public override string ToString()
    {
        return "BoardGameMongos { \n" +
            $"No_of_players = {No_Of_Players}, \n" +
            $"Play_time = {Play_Time}, \n" +
            $"Age_Group = {Age_Group}, \n" +
            $"Item_id = {item_id} \n" +
            "}";
    }
}