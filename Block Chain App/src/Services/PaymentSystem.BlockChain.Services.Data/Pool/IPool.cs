namespace PaymentSystem.BlockChain.Services.Data.Pool
{
    using System.Collections.Generic;

    public interface IPool<T>
    {
        bool Exists(string hash);

        void Add(string hash, T item);

        IEnumerable<T> Get();
    }
}
