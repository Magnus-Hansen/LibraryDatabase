namespace graphMigrator.Models
{
    public class Loaner
    {
        public int Id { get; set; }
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public required string CPR { get; set; }
        public string? Tlf { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
