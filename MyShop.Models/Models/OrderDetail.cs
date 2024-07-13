using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Models
{
	public class OrderDetail
	{
		public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        [ValidateNever]
        public virtual OrderHeader OrderHeader { get; set; }
		[ValidateNever]
		public virtual Product Product { get; set; }

    }
}
