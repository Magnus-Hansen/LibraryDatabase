using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ReviewMongo
{
    [BsonId]
    public ObjectId Review_Id { get; set; }
    public int Loaner_Id { get; set; }
    public string Loaner_Name { get; set; }

    public int No_Of_Stars { get; set; } // 1 to 5
    public string Text { get; set; }

    // Added for mapping for migration service
    public int object_id { get; set; }
}