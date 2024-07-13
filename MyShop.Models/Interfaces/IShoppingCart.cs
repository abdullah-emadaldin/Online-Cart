using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Interfaces
{
	public interface IShoppingCart:IBaseRepository<ShoppingCart>
	{
		public int IncreaseCount(ShoppingCart shoppingCart, int count);
		public int DecreaseCount(ShoppingCart shoppingCart, int count);
	}
}
