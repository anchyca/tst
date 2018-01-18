using SquadLocker.Products.Data.Entities;
using SquadLocker.Products.Data.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SquadLocker.Products.Data
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> Table { get; }

        TEntity FindById(object id);
        Task<TEntity> FindByIdAsync(object id);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None);

        ICollection<TEntity> Get(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None);
        Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None);

        void Create(TEntity entity);
        void Create(IEnumerable<TEntity> entities);

        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);

        void Delete(object id);
        void Delete(TEntity entity);

        void DetachAll();
    }
}
