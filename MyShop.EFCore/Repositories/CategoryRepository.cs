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
	public class CategoryRepository : BaseRepository<Category>, ICategory
	{
		private readonly AppDbContext _context;
		public CategoryRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<int> Update(Category category)
		{
			return await _context.Categories.Where(x=>x.Id==category.Id).ExecuteUpdateAsync(x=>x.SetProperty(z=>z.Name,category.Name).SetProperty(z=>z.Description,category.Description));
		}
	}
}
