using Microsoft.AspNetCore.Mvc;
using MyShop.Model.Interfaces;
using System.Security.Claims;

namespace MyShop.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitofwork;
        public ShoppingCartViewComponent(IUnitOfWork unitofwork)
        {
                _unitofwork = unitofwork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null) 
            {
                if (HttpContext.Session.GetInt32("cart") != null)
                {
                    return View(HttpContext.Session.GetInt32("cart"));
                }
                else
                {
                    HttpContext.Session.SetInt32("cart", _unitofwork.ShoppingCart.GetAll(x => x.UserId == claim.Value).ToList().Count());
                    return View(HttpContext.Session.GetInt32("cart"));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
