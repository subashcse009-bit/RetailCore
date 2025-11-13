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
    public class ProductRepository : Repository<Product>, IProduct
    {
        private readonly ApplicationDBContext _dbContext;
        public ProductRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Product product)
        {
            var lbusProduct = _dbContext.Products.FirstOrDefault(prod => prod.Id == product.Id);
            if (lbusProduct != null)
            {
                lbusProduct.Title = product.Title;
                lbusProduct.ISBN = product.ISBN;
                lbusProduct.Description = product.Description;
                lbusProduct.Price = product.Price;
                lbusProduct.ListPrice = product.ListPrice;
                lbusProduct.CategoryId = product.CategoryId;
                lbusProduct.Description = product.Description;
                lbusProduct.Price50 = product.Price50;
                lbusProduct.Price100 = product.Price100;
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    lbusProduct.ImageUrl = product.ImageUrl;
                }
            }
        }

    }
}
