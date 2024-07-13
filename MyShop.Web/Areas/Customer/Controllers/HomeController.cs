using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.EFCore.Repositories;
using MyShop.Model.Interfaces;
using MyShop.Model.Models;
using MyShop.Model.ViewModels;
using System.Security.Claims;
using X.PagedList;

namespace MyShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Index(int? page)
        {

            var PageNumber = page ?? 1;
            int PageSize = 5;


            var products = await _unitOfWork.Product.GetAllAsync();
            
            return View(products.ToPagedList(PageNumber, PageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Console.WriteLine("i am here");
            if (id <= 0) { return NotFound(); }

            ShoppingCart cart = new ShoppingCart()
            {
                ProductId = id,
                Product = await _unitOfWork.Product.Get(id),
                Count = 1
            };




            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.UserId = claim.Value;

            ShoppingCart cart =  _unitOfWork.ShoppingCart.GetWhere(x=>x.UserId == claim.Value && x.ProductId == shoppingCart.ProductId).FirstOrDefault();
            if (cart == null)
            {
                var newcart = new ShoppingCart()
                {
                    ProductId = shoppingCart.ProductId,
                    UserId = claim.Value,
                    Count = shoppingCart.Count
                };
                await _unitOfWork.ShoppingCart.Add(newcart);

                await _unitOfWork.SaveChangesAsync();

                HttpContext.Session.SetInt32("cart",_unitOfWork.ShoppingCart.GetAll(x => x.UserId == claim.Value).ToList().Count());

            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(cart,shoppingCart.Count);
                await _unitOfWork.SaveChangesAsync();
            }

            
          

            return RedirectToAction("Index");

        }
    }
}
