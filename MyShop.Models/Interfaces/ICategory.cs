using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Interfaces
{
	public interface ICategory:IBaseRepository<Category>
	{
		Task<int> Update(Category category);
	}
}
