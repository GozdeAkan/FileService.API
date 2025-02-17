using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Contracts
{
        public interface IBaseService<TEntity, TDto, TUpdateDto>
        where TEntity : class
        where TDto : class
        where TUpdateDto : class?
        {
            Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

            Task<TEntity> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

            Task CreateAsync(TDto dto);

            Task CreateEntityAsync(TEntity entity);
            Task UpdateAsync(Guid id, TUpdateDto? dto, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

            Task DeleteAsync(Guid id);

            Task UpdateEntityAsync(TEntity entity);
        }
}

