using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Utilities;
using System.Diagnostics;
using System.Security.Claims;

namespace RetailCore.Web.Areas.Customer.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Ok(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new ShoppingCart
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return Ok(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserdId = userId;

            ShoppingCart existingShoppingCart = _unitOfWork.ShoppingCart.Get(s => s.ApplicationUserdId == userId &&
                s.ProductId == shoppingCart.ProductId);

            if (existingShoppingCart != null)
            {
                //Shopping Cart Exists
                existingShoppingCart.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(existingShoppingCart);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(RetailCoreConstants.SessionConstants.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserdId == userId).Count());
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Ok(new RetailCore.Model.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
