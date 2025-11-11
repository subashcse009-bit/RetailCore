using Microsoft.EntityFrameworkCore;
using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _dbContext;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            this.dbSet= _dbContext.Set<T>();
        }
        public void Add(T entity)
        {
           dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
           return dbSet.FirstOrDefault(filter);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
           dbSet.RemoveRange(entity);
        }
    }
}
