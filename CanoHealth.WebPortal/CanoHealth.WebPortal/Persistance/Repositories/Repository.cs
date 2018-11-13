using CanoHealth.WebPortal.Core.Repositories;
using CanoHealth.WebPortal.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IAsyncRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        private DbSet<TEntity> _entities;

        public Repository(DbContext context)
        {
            Context = context;
            _entities = Context.Set<TEntity>();
        }

        protected DbSet<TEntity> Entities
        {
            get
            {
                return Context.Set<TEntity>();
            }
        }

        #region Sync

        public TEntity Get(int id)
        {
            //Here we are working with a DbContext, not an specific Context. So we don't have DbSets<>
            //such as Courses or Authors, and we need to use the Set() method to access them
            //return Context.Set<TEntity>().Find(id);
            return _entities.Find(id);
        }

        public TEntity Get(Guid id)
        {
            return _entities.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            //return Context.Set<TEntity>().ToList();
            return _entities.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            //return Context.Set<TEntity>().Where(predicate);
            return _entities.Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.SingleOrDefault(predicate);
            //return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.FirstOrDefault(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.Any(predicate);
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
            //Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
            //Context.Set<TEntity>().AddRange(entities);
        }

        public void UpdateByGeneric(TEntity entityToUpdate)
        {
            _entities.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            _entities.Remove(entity);
            //Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entities.RemoveRange(entities);
            //Context.Set<TEntity>().RemoveRange(entities);
        }

        public IEnumerable<TEntity> EnumarableGetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _entities;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeExpression in includeProperties)
            {
                query = query.Include(includeExpression);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public IQueryable<TEntity> QueryableGetAll(Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            return query;
        }

        public IEnumerable<TEntity> GetWithRawSqlForTypesAreNotEntities(string query, params object[] parameters)
        {
            return Context.Database.SqlQuery<TEntity>(query, parameters);
        }

        public IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return _entities.SqlQuery(query, parameters).ToList();
        }

        #endregion

        #region Async

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<List<TEntity>> ListAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            IQueryable<TEntity> query = _entities;

            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query.AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            secondaryResult = secondaryResult.Where(spec.Filter);

            if (spec.OrderBy != null)
            {
                secondaryResult = spec.OrderBy(secondaryResult);
            }

            return await secondaryResult.ToListAsync();
        }
        #endregion
    }
}