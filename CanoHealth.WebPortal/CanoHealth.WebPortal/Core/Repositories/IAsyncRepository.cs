using CanoHealth.WebPortal.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(Guid id);

        Task<List<TEntity>> ListAllAsync();

        Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec);

        Task<TEntity> SingleOrDefaultAsync(ISpecification<TEntity> spec);
    }
}
