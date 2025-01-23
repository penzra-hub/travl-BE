using Travl.Domain.Commons;

namespace Travl.Domain.Entities
{
    public class PromoCode : BaseEntity
    {
        public string? Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
