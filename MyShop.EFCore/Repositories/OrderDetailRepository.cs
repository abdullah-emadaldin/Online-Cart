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
	public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetail
	{
		private readonly AppDbContext _context;
		public OrderDetailRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderDetail OrderDetail)
		{
			  _context.OrderDetails.Update(OrderDetail);
		}

		
	}
}
