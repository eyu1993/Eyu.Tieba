using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Eyu.Tieba.IRepository;
using System.Data.Entity;

namespace Eyu.Tieba.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private DbContext _db;
        public GenericRepository(DbContext dbContext)
        {
            this._db = dbContext;
        }
        public void Add(T entity)
        {
            _db.Entry(entity).State = EntityState.Added;
        }
        public void Delete(T entity)
        {
            _db.Entry(entity).State = EntityState.Deleted;
        }
        public void Update(T entity)
        {
            _db.Entry<T>(entity).State = EntityState.Modified;
        }
        public T GetSingle(Expression<Func<T, bool>> lambda)
        {
            return _db.Set<T>().Where(lambda).FirstOrDefault();
        }
        public IQueryable<T> GetMany(Expression<Func<T, bool>> lambda)
        {
            return _db.Set<T>().Where(lambda);
        }
        public IQueryable<T> GetAll()
        {
            return _db.Set<T>();
        }
        public Task<T> GetSingleAsync(Expression<Func<T, bool>> lambda)
        {
            return _db.Set<T>().Where(lambda).FirstOrDefaultAsync();
        }
        public Task<List<T>> GetManyAsync(Expression<Func<T, bool>> lambda)
        {
            return _db.Set<T>().Where(lambda).ToListAsync();
        }
        public Task<List<T>> GetAllAsync()
        {
            return _db.Set<T>().ToListAsync();
        }
    }
}
