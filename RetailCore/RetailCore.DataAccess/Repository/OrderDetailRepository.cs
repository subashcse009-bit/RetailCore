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
    public class OrderDetailRepository : Repository<OrderDetail>,IOrderDetail
    {
        private readonly ApplicationDBContext _dbContext;
        public OrderDetailRepository(ApplicationDBContext dbContext):base (dbContext)
        {
            _dbContext= dbContext;
        }

        public void Update(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Update(orderDetail);
        }
    }
}
