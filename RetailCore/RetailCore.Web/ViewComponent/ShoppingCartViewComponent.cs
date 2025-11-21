using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Utilities;
using Microsoft.AspNetCore.Mvc;
using RetailCore.DataAccess.Repository.IRepository;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {

                if (HttpContext.Session.GetInt32(RetailCoreConstants.SessionConstants.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(RetailCoreConstants.SessionConstants.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserdId == claim.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(RetailCoreConstants.SessionConstants.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}