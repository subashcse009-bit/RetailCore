using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Migrations;
using RetailCore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository
{
    public class ProductRepository : Repository<ProductRepository>, IProduct
    {
        private readonly ApplicationDBContext _dbContext;
        public ProductRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Updaet(Product product)
        {
            _dbContext.Update<Product>(product);
        }

    }
}
