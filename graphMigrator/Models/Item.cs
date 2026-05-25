namespace graphMigrator.Models
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? Release_year { get; set; }
        public string? Description { get; set; }
        public string? Review_summary { get; set; }
        public required string Media_type { get; set; }
        public string? Image { get; set; }
        public required int Language_id { get; set; }
        public required int Publisher_id { get; set; }
        public Decimal? Average_stars { get; set; }

    }
}
