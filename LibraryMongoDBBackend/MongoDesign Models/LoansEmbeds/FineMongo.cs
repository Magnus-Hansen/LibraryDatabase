using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


    public class FineMongo
    {

        [BsonId]
        public ObjectId Id { get; set; }

        public double Amount { get; set; }
        public string Status { get; set; } // "unpaid" | "paid" | "late" Maybe make enum?

        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
    }
