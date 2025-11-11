using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository
{
    public class CategoryRepository:Repository<Category>,ICategory
    {
        private readonly ApplicationDBContext _dbContext;
        public CategoryRepository(ApplicationDBContext dbContext):base (dbContext)
        {
            _dbContext= dbContext;
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
