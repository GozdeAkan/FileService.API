using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Utils
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Retrieves a generic repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// Saves all changes made in the current context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();

    }
}
