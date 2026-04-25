using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class ActiveReservationsPreviewMongo
    {
        [BsonId]
        public ObjectId ReservationId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int QueueNumber { get; set; }
        public string Status { get; set; }

    }

