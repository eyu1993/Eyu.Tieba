using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.IRepository
{
    public interface IGenericRepository<T> where T : class, new()
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);

        T GetSingle(System.Linq.Expressions.Expression<Func<T, bool>> lambda);
        IQueryable<T> GetMany(System.Linq.Expressions.Expression<Func<T, bool>> lambda);
        IQueryable<T> GetAll();

        Task<T> GetSingleAsync(System.Linq.Expressions.Expression<Func<T, bool>> lambda);
        Task<List<T>> GetManyAsync(System.Linq.Expressions.Expression<Func<T, bool>> lambda);
        Task<List<T>> GetAllAsync();
    }
}
