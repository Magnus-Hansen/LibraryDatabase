using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibrarySQLBackend.Models;

[Table("audit_log")]
public partial class AuditLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("table_name")]
    [StringLength(100)]
    public string TableName { get; set; } = null!;

    [Column("record_id")]
    public int RecordId { get; set; }

    [Column("action_type", TypeName = "enum('INSERT','UPDATE','DELETE')")]
    public string ActionType { get; set; } = null!;

    [Column("old_values", TypeName = "json")]
    public string? OldValues { get; set; }

    [Column("new_values", TypeName = "json")]
    public string? NewValues { get; set; }

    [Column("changed_by")]
    [StringLength(100)]
    public string? ChangedBy { get; set; }

    [Column("changed_at", TypeName = "datetime")]
    public DateTime ChangedAt { get; set; }
}
