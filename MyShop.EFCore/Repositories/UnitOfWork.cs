using MyShop.EFCore.Data;
using MyShop.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.EFCore.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;
		public ICategory Category { get; private set; }
		public IProduct Product { get; private set; }
		public IShoppingCart ShoppingCart { get; private set; }
		public IOrderHeader OrderHeader { get; private set; }
		public IOrderDetail OrderDetail { get; private set; }
		public IUser User { get; private set; }
		

		public UnitOfWork(AppDbContext context)
        {
            _context = context;
			Category = new CategoryRepository(context);
			Product = new ProductRepository(context);
			ShoppingCart = new ShoppingCartRepository(context);
			OrderHeader = new OrderHeaderRepository(context);
			OrderDetail = new OrderDetailRepository(context);
			User = new UserRepository(context);
        }

        public void Dispose()
		{
			 _context.Dispose();
		}

		public int SaveChanges()
		{
			return _context.SaveChanges();
		}
		public Task<int> SaveChangesAsync()
		{
			return _context.SaveChangesAsync();
		}
	}
}
