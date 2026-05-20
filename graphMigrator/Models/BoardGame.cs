using Microsoft.Graph.Me.InferenceClassification.Overrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string? No_of_players { get; set; }
        public string? Play_time { get; set; }
        public string? Age_group { get; set; }
        public int Item_id { get; set; }

        public override string ToString()
        {
            return "BoardGame { \n" +
                $"Id = {Id}, \n" +
                $"No_of_players = {No_of_players}, \n" +
                $"Play_time = {Play_time}, \n" +
                $"Age_Group = {Age_group}, \n" +
                $"Item_id = {Item_id} \n" +
                "}";
        }
    }

    
}
