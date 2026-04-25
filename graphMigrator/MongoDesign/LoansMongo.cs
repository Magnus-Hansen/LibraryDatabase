using graphMigrator.MongoDesign.LoansEmbeds;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace graphMigrator.MongoDesign
{
    public class LoansMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId Loaner_Id { get; set; }
        public ObjectId InventoryId { get; set; }

        public DateTime Loan_Date { get; set; }
        public DateTime Due_Date { get; set; }
        public DateTime Return_Date { get; set; }

        public string Status { get; set; } // "active" | "overdue" | "returned"
        public ItemSnapshot Item_Snapshot { get; set; }
        public InventorySnapshot Inventory_Snapshot { get; set; }
        public List<Fine> Fines { get; set; }

    }
}
