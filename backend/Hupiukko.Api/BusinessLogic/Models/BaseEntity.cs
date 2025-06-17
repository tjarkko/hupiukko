using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hupiukko.Api.BusinessLogic.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    // Timestamp for optimistic concurrency control
    // Automatically managed by SQL Server and EF Core
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
} 