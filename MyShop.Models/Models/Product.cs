﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Models
{
	public class Product
	{
		public int Id { get; set; }
		[Required]
		[DisplayName("Category")]
        public int CategoryId { get; set; }
		[Required]
        public string Name { get; set; }
		public string Description { get; set; }
        [DisplayName("Image")]
		[ValidateNever]
        public string Img { get; set; }
		[Required]
		public decimal Price { get; set; }
        [ValidateNever]
        public virtual Category Category { get; set; }
	}
}
