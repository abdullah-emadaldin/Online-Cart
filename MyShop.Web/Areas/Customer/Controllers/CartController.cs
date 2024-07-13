using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Model.Enums;
using MyShop.Model.Interfaces;
using MyShop.Model.Models;
using MyShop.Model.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.AspNetCore.Session;

namespace MyShop.Web.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				CartList = _unitOfWork.ShoppingCart.GetWhere(x => x.UserId == claim.Value)
			};

			foreach (var item in ShoppingCartVM.CartList)
			{
				ShoppingCartVM.Total += item.Count * item.Product.Price;
			}

			return View(ShoppingCartVM);
		}

		public async Task<IActionResult> Plus(int cartid)
		{
			var cart = await _unitOfWork.ShoppingCart.Get(cartid);
			if (cart == null) { return NotFound(); }
			_unitOfWork.ShoppingCart.IncreaseCount(cart, 1);
			await _unitOfWork.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Minus(int cartid)
		{
			var cart = await _unitOfWork.ShoppingCart.Get(cartid);
			if (cart == null) { return NotFound(); }
			if (cart.Count <= 1)
			{
				await _unitOfWork.ShoppingCart.Remove(cartid);
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.UserId == cart.UserId).ToList().Count() - 1;
                HttpContext.Session.SetInt32("cart", count);		
			}
			else
			{
				_unitOfWork.ShoppingCart.DecreaseCount(cart, 1);
			}

			await _unitOfWork.SaveChangesAsync();
			return RedirectToAction("Index");
		}



		public async Task<IActionResult> Remove(int cartid)
		{
			var cart = await _unitOfWork.ShoppingCart.Get(cartid);
			if (cart == null) { return NotFound(); }
			await _unitOfWork.ShoppingCart.Remove(cartid);
			await _unitOfWork.SaveChangesAsync();
            var count = _unitOfWork.ShoppingCart.GetAll(x => x.UserId == cart.UserId).ToList().Count();
            HttpContext.Session.SetInt32("cart", count);
            return RedirectToAction("Index");
		}


		public IActionResult Summary()
		{
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity!;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

				ShoppingCartVM = new ShoppingCartVM()
				{
					CartList = _unitOfWork.ShoppingCart.GetWhere(x => x.UserId == claim.Value),
					OrderHeader = new()
				};

				ShoppingCartVM.OrderHeader.User = _unitOfWork.User.GetWhere(x => x.Id == claim.Value).FirstOrDefault()!;
				ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.User.FirstName + ShoppingCartVM.OrderHeader.User.LastName;
				ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.User.Address;
				ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.User.City;
				ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.User.PhoneNumber;

				foreach (var item in ShoppingCartVM.CartList)
				{
					ShoppingCartVM.OrderHeader.Total += item.Count * item.Product.Price;
				}

				return View(ShoppingCartVM);
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Summary")]
		public async Task<IActionResult> Summary(ShoppingCartVM shoppingCartVM)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCartVM.CartList = _unitOfWork.ShoppingCart.GetWhere(x => x.UserId == claim.Value);

			shoppingCartVM.OrderHeader.OrderStatus = OrderSatues.Pending.ToString();
			shoppingCartVM.OrderHeader.PaymentStatus = OrderSatues.Pending.ToString();
			shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			shoppingCartVM.OrderHeader.UserId = claim.Value;
			foreach (var item in shoppingCartVM.CartList)
			{
				shoppingCartVM.OrderHeader.Total += item.Count * item.Product.Price;
			}
			await _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
			await _unitOfWork.SaveChangesAsync();

			foreach (var item in shoppingCartVM.CartList)
			{
				OrderDetail orderDetail = new OrderDetail()
				{
					ProductId = item.ProductId,
					OrderHeaderId = shoppingCartVM.OrderHeader.Id,
					Price = item.Product.Price,
					Count = item.Count
				};

				await _unitOfWork.OrderDetail.Add(orderDetail);
				await _unitOfWork.SaveChangesAsync();
			}
       
              // var domain = "https://localhost:7223/";
            var domain = "http://online-cart.runasp.net/";
            var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),

				Mode = "payment",
				SuccessUrl = domain + $"customer/cart/orderconfirmation?id={shoppingCartVM.OrderHeader.Id}",
				CancelUrl = domain + $"customer/cart/index",
			};

			foreach (var item in shoppingCartVM.CartList)
			{
				var sessionlineoption = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionlineoption);
			}


			var service = new SessionService();
			Session session = service.Create(options);
			shoppingCartVM.OrderHeader.SessionId = session.Id;

			await _unitOfWork.SaveChangesAsync();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

			//_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.CartsList);
			//         _unitOfWork.Complete();
			//         return RedirectToAction("Index","Home");
		}




		public async Task<IActionResult> OrderConfirmationAsync(int id)
		{
			OrderHeader orderHeader = await _unitOfWork.OrderHeader.Get(id);
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);

			if (session.PaymentStatus.ToLower() == "paid")
			{
				await _unitOfWork.OrderHeader.UpdateStatusAsync(id, OrderSatues.Approved.ToString(), OrderSatues.Approved.ToString());
				orderHeader.PaymentIntentId = session.PaymentIntentId;
				await _unitOfWork.SaveChangesAsync();
			}
			List<ShoppingCart> shoppingcarts =  _unitOfWork.ShoppingCart.GetAll(u => u.UserId == orderHeader.UserId).ToList();
			HttpContext.Session.Clear();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingcarts);
			await _unitOfWork.SaveChangesAsync();
			return View(id);




		}
	}
}
