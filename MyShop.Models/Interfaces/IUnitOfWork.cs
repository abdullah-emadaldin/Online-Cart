using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Interfaces
{
	public interface IUnitOfWork:IDisposable
	{
		ICategory Category { get; }
		IProduct Product { get; }
		IShoppingCart ShoppingCart { get; }
		IOrderHeader OrderHeader { get; }
		IOrderDetail OrderDetail { get; }
		IUser User { get; }

		int SaveChanges();
		Task<int> SaveChangesAsync();
	}
}
