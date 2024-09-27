 
namespace Empresa.Inv.EntityFrameworkCore
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task<int> SaveAsync();
    }
}
