using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public string? WalletId { get; set; }
        public Wallet? Wallet { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNo { get; set; }
        public RecepientAccountType RecepientAccountType { get; set; }
        public string? RecepientAccount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}