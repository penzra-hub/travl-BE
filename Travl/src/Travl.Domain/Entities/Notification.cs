using Travl.Domain.Commons;

namespace Travl.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string? AppUserId { get; set; }
        public AppUser? User { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; }
    }
}
