using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Models
{
	public class ShoppingCart
	{
		public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }

		[Range(1,100,ErrorMessage ="you must enter value 1 or more")]
		public int Count { get; set; }
		[ForeignKey("UserId")]
		[ValidateNever]
		public virtual User User { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public virtual Product Product { get; set; }
		
	}
}
