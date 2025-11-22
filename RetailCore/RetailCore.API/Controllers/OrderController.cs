using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Model.ViewModel;
using RetailCore.Utilities;
using System.Security.Claims;

namespace RetailCore.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin)]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return Ok();
        }

        public IActionResult Detail(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(g => g.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(g => g.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return Ok(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin + "," + RetailCoreConstants.Roles.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var lbusOrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            lbusOrderHeader.Name = OrderVM.OrderHeader.Name;
            lbusOrderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            lbusOrderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            lbusOrderHeader.City = OrderVM.OrderHeader.City;
            lbusOrderHeader.State = OrderVM.OrderHeader.State;
            lbusOrderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                lbusOrderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                lbusOrderHeader.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(lbusOrderHeader);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Detail), new { orderId = lbusOrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin + "," + RetailCoreConstants.Roles.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, RetailCoreConstants.OrderStatus.StatusInProcess);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin + "," + RetailCoreConstants.Roles.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = RetailCoreConstants.OrderStatus.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == RetailCoreConstants.PaymentStatus.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin + "," + RetailCoreConstants.Roles.Role_Employee)]
        public IActionResult CancelOrder()
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderHeader.PaymentStatus == RetailCoreConstants.PaymentStatus.PaymentStatusApproved)
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, RetailCoreConstants.OrderStatus.StatusCancelled, RetailCoreConstants.OrderStatus.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, RetailCoreConstants.OrderStatus.StatusCancelled, RetailCoreConstants.OrderStatus.StatusCancelled);
            }
            _unitOfWork.Save();
            return Ok();
        }


        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader
                .Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail
                .GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            var sessionId = Guid.NewGuid().ToString();
            var paymentIntentId = Guid.NewGuid().ToString();
            _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, sessionId, paymentIntentId);
            _unitOfWork.Save();

            return RedirectToAction(nameof(PaymentConfirmation), new { id = OrderVM.OrderHeader.Id });
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == RetailCoreConstants.PaymentStatus.PaymentStatusDelayedPayment)
            {
                //this is an order by company
                var sessionId = Guid.NewGuid().ToString();
                var paymentIntentId = Guid.NewGuid().ToString();

                _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, sessionId, paymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, RetailCoreConstants.PaymentStatus.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            return Ok(orderHeaderId);
        }

        #region API CALLS
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> lstorderHeaders = null;

            if (User.IsInRole(RetailCoreConstants.Roles.Role_Admin) || User.IsInRole(RetailCoreConstants.Roles.Role_Employee))
            {
                lstorderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                lstorderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

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

            return Ok();
        }
        #endregion
    }
}
