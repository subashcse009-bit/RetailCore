using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDBContext _dbContext;
        public ICategory Category { get; private set; }
        public IProduct Product { get; private set; }
        public UnitOfWork(ApplicationDBContext dbContext)
        {
            _dbContext= dbContext;
            Category= new CategoryRepository(_dbContext);
            Product = new ProductRepository(_dbContext);
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
