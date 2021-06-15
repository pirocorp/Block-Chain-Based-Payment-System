namespace PaymentSystem.Common.Hubs.Models
{
    using System.Collections.Generic;

    public class NotificationBlock
    {
        public string Hash { get; set; }

        public long Height { get; set; }

        public NotificationHeader BlockHeader { get; set; }

        public IEnumerable<TransactionNotification> Transactions { get; set; }

        public string Validator { get; set; }

        public string BlockChainPublicKey { get; set; }

        // Signature will be made over block hash.
        public string BlockChainSignature { get; set; }
    }
}
