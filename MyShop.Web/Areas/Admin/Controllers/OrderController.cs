using Microsoft.AspNetCore.Mvc;
using MyShop.Model.Interfaces;
using MyShop.Model.Models;
using Stripe.Climate;
using Stripe;
using MyShop.Model.ViewModels;
using MyShop.Model.Enums;
using Microsoft.AspNetCore.Authorization;

namespace MyShop.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = await _unitofwork.OrderHeader.GetAllAsync();
            return Json(new { data = orderHeaders });
        }

        public async Task<IActionResult> Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
				OrderHeader = await _unitofwork.OrderHeader.Get(orderid),
                OrderDetails = await _unitofwork.OrderDetail.GetAllAsync(x => x.OrderHeaderId == orderid)
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetailsAsync()
        {
			var orderfromdb = await _unitofwork.OrderHeader.Get(OrderVM.OrderHeader.Id);
            orderfromdb.Name = OrderVM.OrderHeader.Name;
            orderfromdb.Phone = OrderVM.OrderHeader.Phone;
            orderfromdb.Address = OrderVM.OrderHeader.Address;
            orderfromdb.City = OrderVM.OrderHeader.City;

            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderfromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitofwork.OrderHeader.Update(orderfromdb);
            await _unitofwork.SaveChangesAsync();
            TempData["Update"] = "Item has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderfromdb.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProccessAsync()
        {
            await _unitofwork.OrderHeader.UpdateStatusAsync(OrderVM.OrderHeader.Id, OrderSatues.Processing.ToString(), null);
            await _unitofwork.SaveChangesAsync();

            TempData["Update"] = "Order Status has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShipAsync()
        {
            var orderfromdb = await _unitofwork.OrderHeader.Get(OrderVM.OrderHeader.Id);
            orderfromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
            orderfromdb.OrderStatus = OrderSatues.Shipped.ToString();
            orderfromdb.ShippingDate = DateTime.Now;

            _unitofwork.OrderHeader.Update(orderfromdb);
			await _unitofwork.SaveChangesAsync();

			TempData["Update"] = "Order has Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrderAsync()
        {
            var orderfromdb = await _unitofwork.OrderHeader.Get(OrderVM.OrderHeader.Id);
            if (orderfromdb.PaymentStatus == OrderSatues.Approved.ToString())
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderfromdb.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(option);

                await _unitofwork.OrderHeader.UpdateStatusAsync(orderfromdb.Id, OrderSatues.Cancelled.ToString(), OrderSatues.Refund.ToString());
            }
            else
            {
                await _unitofwork.OrderHeader.UpdateStatusAsync(orderfromdb.Id, OrderSatues.Cancelled.ToString(), OrderSatues.Cancelled.ToString());
            }
            await _unitofwork.SaveChangesAsync();

            TempData["Update"] = "Order has Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }
    }
}
