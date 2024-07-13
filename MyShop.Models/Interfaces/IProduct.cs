using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Interfaces
{
	public interface IProduct:IBaseRepository<Product>
	{
		Task<int> Update(Product category);
	}
}
