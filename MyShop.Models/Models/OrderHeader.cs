using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        // Stripe Properties
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set;}
        [ValidateNever]
        public virtual User User { get; set; }
		public string Name { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
        public string? Phone { get; set; } 
	}
}
