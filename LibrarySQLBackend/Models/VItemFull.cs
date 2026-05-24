using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibrarySQLBackend.Models;

[Keyless]
public partial class VItemFull
{
    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("release_year")]
    public ushort? ReleaseYear { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("review_summary", TypeName = "text")]
    public string? ReviewSummary { get; set; }

    [Column("media_type", TypeName = "enum('book','boardgame')")]
    public string MediaType { get; set; } = null!;

    [Column("image")]
    [StringLength(2048)]
    public string? Image { get; set; }

    [Column("average_stars")]
    [Precision(2, 1)]
    public decimal? AverageStars { get; set; }

    [Column("language_name")]
    [StringLength(45)]
    public string LanguageName { get; set; } = null!;

    [Column("publisher_name")]
    [StringLength(70)]
    public string PublisherName { get; set; } = null!;

    [Column("ISBN")]
    [StringLength(17)]
    public string? Isbn { get; set; }

    [Column("no_of_pages")]
    public ushort? NoOfPages { get; set; }

    [Column("version")]
    [StringLength(45)]
    public string? Version { get; set; }

    [Column("no_of_players")]
    [StringLength(20)]
    public string? NoOfPlayers { get; set; }

    [Column("play_time")]
    [StringLength(20)]
    public string? PlayTime { get; set; }

    [Column("age_group")]
    [StringLength(20)]
    public string? AgeGroup { get; set; }

    [Column("creators", TypeName = "text")]
    public string? Creators { get; set; }

    [Column("genres", TypeName = "text")]
    public string? Genres { get; set; }

    [Column("tags", TypeName = "text")]
    public string? Tags { get; set; }
}
