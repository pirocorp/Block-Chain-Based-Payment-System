namespace PaymentSystem.Common.Hubs.Models
{
    using System.Collections.Generic;

    using AutoMapper;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Mapping;

    public class NotificationBlock : IMapFrom<Block>
    {
        public string Hash { get; set; }

        public long Height { get; set; }

        public NotificationHeader BlockHeader { get; set; }

        public IEnumerable<TransactionNotification> Transactions { get; set; }

        public string Validator { get; set; }

        [IgnoreMap]
        public string BlockChainPublicKey { get; set; }

        // Signature will be made over block hash.
        [IgnoreMap]
        public string BlockChainSignature { get; set; }

        [IgnoreMap]
        public IEnumerable<TransactionNotification> CanceledTransactions { get; set; }
    }
}
