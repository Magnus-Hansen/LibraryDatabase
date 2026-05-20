using LibraryMongoDBBackend.MongoDesign_Models.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ItemMongo : IMongoModel
{
    [BsonId]
    public ObjectId _id { get; set; }
    public int? Id { get; set; }
    public string Name { get; set; }
    public string MediaType { get; set; } // "book" | "boardgame"

    public int? ReleaseYear { get; set; }
    public string Description { get; set; }
    public string ReviewSummary { get; set; }
    public string Image { get; set; }

    public LanguageMongo Language { get; set; }
    public PublisherMongo Publisher { get; set; }

    public List<CreatorMongo> Creators { get; set; }
    public List<GenreMongo> Genres { get; set; }
    public List<TagMongo> Tags { get; set; }

    public BookDetailsMongo? BookDetails { get; set; } // nullable
    public BoardgameDetailsMongo? BoardgameDetails { get; set; } // nullable

    public List<ReviewMongo> Reviews { get; set; }

    public double AverageStars { get; set; }
}