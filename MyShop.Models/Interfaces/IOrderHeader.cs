﻿using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Interfaces
{
	public interface IOrderHeader:IBaseRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);
		 Task UpdateStatusAsync(int id, string orderstatus, string paymentstatus);
	}
}
