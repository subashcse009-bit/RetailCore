using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Migrations;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProduct
    {
        private readonly ApplicationDBContext _dbContext;
        public ProductRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Product product)
        {
            _dbContext.Products.Update(product);
        }

    }
}
