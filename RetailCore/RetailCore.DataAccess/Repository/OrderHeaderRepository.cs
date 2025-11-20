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
    public class OrderHeaderRepository : Repository<OrderHeader>,IOrderHeader
    {
        private readonly ApplicationDBContext _dbContext;
        public OrderHeaderRepository(ApplicationDBContext dbContext):base (dbContext)
        {
            _dbContext= dbContext;
        }

        public void Update(OrderHeader orderHeader)
        {
            _dbContext.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var lbusOrderHeader = _dbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (lbusOrderHeader != null)
            {
                lbusOrderHeader.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    lbusOrderHeader.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var lbusOrderHeader = _dbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                lbusOrderHeader.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                lbusOrderHeader.PaymentIntentId = paymentIntentId;
                lbusOrderHeader.PaymentDate = DateTime.Now;
            }
        }
    }
}
