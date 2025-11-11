using RetailCore.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository.IRepository
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDBContext _dbContext;
        public ICategory Category { get; private set; }
        public UnitOfWork(ApplicationDBContext dbContext)
        {
            _dbContext= dbContext;
            Category= new CategoryRepository(_dbContext);
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
