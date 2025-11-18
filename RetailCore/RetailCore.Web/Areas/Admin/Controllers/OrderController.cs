using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Model.ViewModel;
using RetailCore.Utilities;

namespace RetailCore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int orderId)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(g => g.Id == orderId),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(g => g.OrderHeaderId == orderId)
            };

            return View(orderVM);
        }


        #region API CALLS
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> lstorderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    lstorderHeaders = lstorderHeaders.Where(u => u.PaymentStatus == RetailCoreConstants.PaymentStatus.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    lstorderHeaders = lstorderHeaders.Where(u => u.OrderStatus == RetailCoreConstants.OrderStatus.StatusInProcess);
                    break;
                case "completed":
                    lstorderHeaders = lstorderHeaders.Where(u => u.OrderStatus == RetailCoreConstants.OrderStatus.StatusShipped);
                    break;
                case "approved":
                    lstorderHeaders = lstorderHeaders.Where(u => u.OrderStatus == RetailCoreConstants.OrderStatus.StatusApproved);
                    break;
                default:
                    break;

            }

            return Json(new { data = lstorderHeaders });
        }
        #endregion
    }
}
