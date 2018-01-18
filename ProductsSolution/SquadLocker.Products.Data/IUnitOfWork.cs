using SquadLocker.Products.Data.Entities;
using System;
using System.Threading.Tasks;

namespace SquadLocker.Products.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        int ExecuteSqlCommand(string sql, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
        int Commit();
        Task<int> CommitAsync();
    }
}
