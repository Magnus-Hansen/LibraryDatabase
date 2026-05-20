using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class ReservationsMongo
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public int Id { get; set; } // This is the Id from the SQL database, not the MongoDB _id
        public int Loaner_Id { get; set; }
        public int Item_Id { get; set; }

        public string Item_Name { get; set; }
        public DateTime Created_At { get; set; }
        public string Status { get; set; } // "pending" | "ready for pickup" | "fulfilled" Maybe make enum?

    }

