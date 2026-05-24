using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibrarySQLBackend.Models;

[Table("language")]
[Index("Language1", Name = "language_UNIQUE", IsUnique = true)]
public partial class Language
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("language")]
    [StringLength(45)]
    public string Language1 { get; set; } = null!;

    [InverseProperty("Language")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
