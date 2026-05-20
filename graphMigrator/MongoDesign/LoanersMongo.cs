using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class LoanersMongo
    {
        [BsonId]
        public ObjectId _id { get; set; }   
        public int Id { get; set; } // This is the Id from the SQL database, not the MongoDB _id

    public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Cpr { get; set; }
        public string Tlf { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public List<ActiveReservationsPreviewMongo> ActiveReservations { get; set; }
        public List<ActiveLoansPreviewMongo> ActiveLoans { get; set; }

    }

