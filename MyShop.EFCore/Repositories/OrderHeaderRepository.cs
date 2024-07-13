using Microsoft.EntityFrameworkCore;
using MyShop.EFCore.Data;
using MyShop.Model.Interfaces;
using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.EFCore.Repositories
{
	public class OrderHeaderRepository : BaseRepository<OrderHeader>, IOrderHeader
	{
		private readonly AppDbContext _context;
		public OrderHeaderRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderHeader OrderHeader)
		{
			  _context.OrderHeaders.Update(OrderHeader);
		}

		public async Task UpdateStatusAsync(int id, string orderstatus, string paymentstatus)
		{
			var order = await _context.OrderHeaders.FindAsync(id);
			if (order != null) 
			{
				order.OrderStatus = orderstatus;
				order.PaymentDate = DateTime.Now;
				if (paymentstatus != null)
					order.PaymentStatus = paymentstatus;
			}
		}
	}
}
