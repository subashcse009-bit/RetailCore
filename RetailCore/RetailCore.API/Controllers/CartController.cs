using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Model.ViewModel;
using RetailCore.Utilities;
using System.Numerics;
using System.Security.Claims;

namespace RetailCore.Web.Areas.Customer.Controllers
{
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new ShoppingCartVM
            {
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserdId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return Ok(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserdId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return Ok(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserdId == userId, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser lbusApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (lbusApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is regular user
                ShoppingCartVM.OrderHeader.PaymentStatus = RetailCoreConstants.PaymentStatus.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = RetailCoreConstants.OrderStatus.StatusPending;
            }
            else
            {
                //it is company user
                ShoppingCartVM.OrderHeader.PaymentStatus = RetailCoreConstants.PaymentStatus.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = RetailCoreConstants.OrderStatus.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
            }
            _unitOfWork.Save();

            if (lbusApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer and we need to capture the payment 
                //stripe logiic 

                var sessionId = Guid.NewGuid().ToString();

                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, sessionId, ShoppingCartVM.OrderHeader.PaymentIntentId);

                _unitOfWork.Save();
            }

            return Ok(ShoppingCartVM);
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader lbusOrderHeader = _unitOfWork.OrderHeader.Get(g => g.Id == id, includeProperties: "ApplicationUser");

            if (lbusOrderHeader != null && lbusOrderHeader.PaymentStatus != RetailCoreConstants.PaymentStatus.PaymentStatusDelayedPayment)
            {
                var paymentIntentId = Guid.NewGuid().ToString();

                _unitOfWork.OrderHeader.UpdateStripePaymentID(id, lbusOrderHeader.SessionId, paymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, RetailCoreConstants.OrderStatus.StatusApproved, RetailCoreConstants.PaymentStatus.PaymentStatusApproved);
                _unitOfWork.Save();

                HttpContext.Session.Clear();
            }

            List<ShoppingCart> lstShoppingCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserdId == lbusOrderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(lstShoppingCart);
            _unitOfWork.Save();

           return Ok(id);
        }

        public IActionResult Plus(int? cartId)
        {
            ShoppingCart ldoshoppingCart = _unitOfWork.ShoppingCart.Get(g => g.Id == cartId);
            ldoshoppingCart.Count += 1;
            _unitOfWork.ShoppingCart.Update(ldoshoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int? cartId)
        {
            ShoppingCart ldoshoppingCart = _unitOfWork.ShoppingCart.Get(g => g.Id == cartId, tracked: true);

            if (ldoshoppingCart.Count <= 1)
            {
                HttpContext.Session.SetInt32(RetailCoreConstants.SessionConstants.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserdId == ldoshoppingCart.ApplicationUserdId).Count() - 1);

                _unitOfWork.ShoppingCart.Remove(ldoshoppingCart);
            }
            else
            {
                ldoshoppingCart.Count -= 1;
                _unitOfWork.ShoppingCart.Update(ldoshoppingCart);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int? cartId)
        {
            ShoppingCart ldoshoppingCart = _unitOfWork.ShoppingCart.Get(g => g.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(RetailCoreConstants.SessionConstants.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserdId == ldoshoppingCart.ApplicationUserdId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(ldoshoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
