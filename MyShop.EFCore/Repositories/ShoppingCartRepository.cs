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
	public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCart
	{
		private readonly AppDbContext _context;
		public ShoppingCartRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public int IncreaseCount(ShoppingCart shoppingCart, int count)
		{
			shoppingCart.Count += count;
			return shoppingCart.Count;
		}
		public int DecreaseCount(ShoppingCart shoppingCart, int count)
		{
			shoppingCart.Count -= count;
			return shoppingCart.Count;
		}
	}
}
