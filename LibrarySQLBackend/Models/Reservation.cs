using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibrarySQLBackend.Models;

[Table("reservation")]
[Index("ItemId", Name = "item_id_idx")]
[Index("LoanerId", Name = "loaner_id_idx")]
[Index("ItemId", "QueueNumber", Name = "uq_reservation_item_queue", IsUnique = true)]
public partial class Reservation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("loaner_id")]
    public int LoanerId { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("status", TypeName = "enum('pending','ready for pickup','fulfilled')")]
    public string Status { get; set; } = null!;

    [Column("queue_number")]
    public uint QueueNumber { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("Reservations")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("LoanerId")]
    [InverseProperty("Reservations")]
    public virtual Loaner Loaner { get; set; } = null!;
}
