using SquadLocker.Products.Data.Entities;
using System;
using System.Collections;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SquadLocker.Products.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Hashtable _repositories;
        private readonly DbContext _dbContext;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);
            var typeName = type.Name;

            if (!_repositories.ContainsKey(typeName))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                             .MakeGenericType(typeof(TEntity)), _dbContext);

                _repositories.Add(typeName, repositoryInstance);
            }

            return (IRepository<TEntity>)_repositories[typeName];
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }
        public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();

            //example how to catch EntityValidationErrors
            //try
            //{
            //    return _dbContext.SaveChanges();
            //}
            //catch (DbEntityValidationException exception)
            //{
            //    foreach (var validationErrors in exception.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
            //        }
            //    }
            //    return -1;
            //}
        }

        public Task<int> CommitAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        #region IDisposable

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
