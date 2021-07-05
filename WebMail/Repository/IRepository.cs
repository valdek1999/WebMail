using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMail.Repository
{
    /// <summary>
    /// Interface for implementing database context management
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity: class
    {
        IEnumerable<TEntity> All { get;}

        TEntity Add(TEntity entity);

        void Delete(TEntity entity);

        void Update(TEntity entity);

        TEntity FindById(int Id);

        IEnumerable<TEntity> FindAll(Func<TEntity,bool> predicate);
    }
}
