using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Model.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		IEnumerable<T> GetAll(Expression<Func<T,bool>>? predicate = null);
		Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
		Task<T> Get(int id);
		Task Add(T entity);
		Task Remove(int id);
		void RemoveRange(IEnumerable<T> entities);
        IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate);

	}
}
