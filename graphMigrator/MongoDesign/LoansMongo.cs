using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


    public class LoansMongo
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public int Id { get; set; } // This is the Id from the SQL database, not the MongoDB _id
    public int Loaner_Id { get; set; }
        public int InventoryId { get; set; }

        public DateTime Loan_Date { get; set; }
        public DateTime Due_Date { get; set; }
        public DateTime? Return_Date { get; set; }

        public string Status { get; set; } // "active" | "overdue" | "returned"
        public ItemSnapshot Item_Snapshot { get; set; }
        public InventorySnapshot Inventory_Snapshot { get; set; }
        public List<FineMongo> Fines { get; set; }

    }

