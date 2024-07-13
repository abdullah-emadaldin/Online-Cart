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
	public class ProductRepository : BaseRepository<Product>, IProduct
	{
		private readonly AppDbContext _context;
		public ProductRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<int> Update(Product product)
		{
			return await _context.Products.Where(x => x.Id == product.Id).ExecuteUpdateAsync(x =>
			x.SetProperty(z => z.Name, product.Name)
			.SetProperty(z => z.Description, product.Description)
			.SetProperty(z => z.Price, product.Price)
			.SetProperty(z => z.Img, product.Img)
            .SetProperty(z => z.CategoryId, product.CategoryId)

			);

		}
	}
}
