using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.MongoDesign
{
    public class InventoryMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonId]
        public ObjectId item_id { get; set; }

        public string item_name { get; set; }
        public string barcode { get; set; }
        public string status { get; set; } // "available" | "loaned out" | "lost" Make enum?
        public string placement { get; set; }

    }
}
