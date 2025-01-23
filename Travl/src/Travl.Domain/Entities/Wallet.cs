using Travl.Domain.Commons;

namespace Travl.Domain.Entities
{
    public class Wallet : BaseEntity//, IEquatable<Wallet>
    {
        public string? AppUserId {  get; set; }
        public AppUser? User { get; set; }
        public string? AccountNo { get; set; }
        public decimal? Balance { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }

        //public bool Equals(Wallet? other)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
