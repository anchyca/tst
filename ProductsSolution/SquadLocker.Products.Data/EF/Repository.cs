using SquadLocker.Products.Data.Entities;
using SquadLocker.Products.Data.Options;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SquadLocker.Products.Data.EF
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public IQueryable<TEntity> Table => _dbSet.AsNoTracking();

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _dbSet = dbContext.Set<TEntity>();
        }

        public virtual TEntity FindById(object id)
        {
            return _dbSet.Find(id);
        }
        public virtual Task<TEntity> FindByIdAsync(object id)
        {
            return _dbSet.FindAsync(id);
        }
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None)
        {
            var query = _dbSet.AsQueryable();
            if (options == GetOptionsEnum.NoTracking)
                query = query.AsNoTracking();

            return query.FirstOrDefault(predicate);
        }
        public virtual Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None)
        {
            var query = _dbSet.AsQueryable();
            if (options == GetOptionsEnum.NoTracking)
                query = query.AsNoTracking();

            return query.FirstOrDefaultAsync(predicate);
        }

        public virtual ICollection<TEntity> Get(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None)
        {
            var query = _dbSet.AsQueryable();
            if (options == GetOptionsEnum.NoTracking)
                query = query.AsNoTracking();

            return query.Where(predicate).ToList();
        }
        public virtual async Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, GetOptionsEnum options = GetOptionsEnum.None)
        {
            var query = _dbSet.AsQueryable();
            if (options == GetOptionsEnum.NoTracking)
                query = query.AsNoTracking();

            var list = (ICollection<TEntity>)await query.Where(predicate).ToListAsync();
            return list;
        }

        public virtual void Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var type = typeof(TEntity);
            if (type.GetProperties().Any(x => x.Name == "Created"))
            {
                var dateCreatedUtcProperty = type.GetProperty("Created");
                var dateCreatedUtc = dateCreatedUtcProperty.GetValue(entity) as DateTime?;
                if (!dateCreatedUtc.HasValue || dateCreatedUtc.Value == DateTime.MinValue || dateCreatedUtc.Value == default(DateTime))
                {
                    dateCreatedUtcProperty.SetValue(entity, DateTime.UtcNow);
                }
            }

            _dbSet.Add(entity);
        }

        public virtual void Create(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            var dbEntityEntry = _dbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
            var type = typeof(TEntity);
            if (type.GetProperties().Any(x => x.Name == "DateCreatedUtc"))
            {
                _dbContext.Entry(entity).Property("DateCreatedUtc").IsModified = false;
            }
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public virtual void Delete(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var type = typeof(TEntity);
            if (type.GetProperties().Any(x => x.Name == "IsDeleted"))
            {
                var isDeleted = type.GetProperty("IsDeleted");
                isDeleted.SetValue(entity, true);
            }
            else
            {
                var dbEntityEntry = _dbContext.Entry(entity);
                if (dbEntityEntry.State != EntityState.Deleted)
                {
                    dbEntityEntry.State = EntityState.Deleted;
                }
                else
                {
                    _dbSet.Attach(entity);
                    _dbSet.Remove(entity);
                }
            }
        }

        public virtual void DetachAll()
        {
            //clear context attached regions before everything
            _dbSet.Local.ToList().ForEach(x =>
            {
                _dbContext.Entry(x).State = EntityState.Detached;
                x = null;
            });
        }


    }
}
