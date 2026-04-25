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
        public ObjectId Id { get; set; }
        public ObjectId Loaner_Id { get; set; }
        public ObjectId Item_Id { get; set; }

        public string Item_Name { get; set; }
        public DateTime Created_At { get; set; }
        public string Status { get; set; } // "pending" | "ready for pickup" | "fulfilled" Maybe make enum?

    }

