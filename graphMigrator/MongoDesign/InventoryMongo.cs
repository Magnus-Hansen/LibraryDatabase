using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class InventoryMongo
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public int Item_Id { get; set; }

        public string Item_Name { get; set; }
        public string Barcode { get; set; }
        public string Status { get; set; } // "available" | "loaned out" | "lost" Make enum?
        public string Placement { get; set; } 

    }

