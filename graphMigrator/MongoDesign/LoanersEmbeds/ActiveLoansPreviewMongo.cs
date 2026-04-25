using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class ActiveLoansPreviewMongo
    {
        [BsonId]
        public ObjectId LoanId { get; set; }
        
        public string Item_Name { get; set; }
        public DateTime Due_Date { get; set; }
    }

