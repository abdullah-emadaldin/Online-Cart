using Microsoft.EntityFrameworkCore;
using MyShop.EFCore.Data;
using MyShop.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.EFCore.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		private readonly AppDbContext _context;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(T entity)
		{
			await _context.AddAsync(entity);
		}

		public async Task<T> Get(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public  IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null)
		{

			IQueryable<T> query = _context.Set<T>();

			if(predicate!= null) {  query = query.Where(predicate); }
			
			return  query;
		}
        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
		{

			IQueryable<T> query = _context.Set<T>();

			if (predicate != null) { query = query.Where(predicate); }

			return await query.ToListAsync();
		}

		public async Task Remove(int id)
		{
			 var item = await _context.Set<T>().FindAsync(id);
			if (item != null)
			{
				_context.Set<T>().Remove(item);
			}
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			_context.Set<T>().RemoveRange(entities);
		}
	}
}
