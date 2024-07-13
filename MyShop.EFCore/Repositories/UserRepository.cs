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
	public class UserRepository : BaseRepository<User>, IUser
	{
		private readonly AppDbContext _context;
		public UserRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		
	}
}
