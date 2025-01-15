using System.ComponentModel.DataAnnotations;
using Travl.Domain.Enums;

namespace Travl.Domain.Commons
{
    public abstract class BaseEntity
    {
        [Key] public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public Status? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
