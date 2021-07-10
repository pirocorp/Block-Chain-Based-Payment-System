namespace PaymentSystem.WalletApp.Data.Common.Repositories
{
    using System.Linq;

    using PaymentSystem.WalletApp.Data.Common.Models;

    public interface IDeletableEntityRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllAsNoTrackingWithDeleted();

        void HardDelete(TEntity entity);

        void UnDelete(TEntity entity);
    }
}
