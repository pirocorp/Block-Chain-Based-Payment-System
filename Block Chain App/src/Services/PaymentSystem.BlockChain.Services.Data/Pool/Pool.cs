namespace PaymentSystem.BlockChain.Services.Data.Pool
{
    using System.Collections.Generic;
    using System.Linq;

    public class Pool<T> : IPool<T>
    {
        private readonly IDictionary<string, T> pool;

        public Pool()
        {
            this.pool = new Dictionary<string, T>();
        }

        public bool Exists(string hash) => this.pool.ContainsKey(hash);

        public void Add(string hash, T item)
            => this.pool.Add(hash, item);

        public IEnumerable<T> Get()
        {
            var transactions = this.pool.ToList();

            transactions
                .ForEach(t => this.pool.Remove(t.Key));

            return transactions.Select(t => t.Value);
        }
    }
}
