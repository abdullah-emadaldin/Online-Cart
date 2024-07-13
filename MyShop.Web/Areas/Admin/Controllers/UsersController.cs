using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.EFCore.Data;
using MyShop.Model.Enums;
using MyShop.Model.Interfaces;
using MyShop.Model.ViewModels;
using System.Data;
using System.Security.Claims;

namespace MyShop.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
          

			return View();
     
        
        
        }

		public async Task<IActionResult> GetData()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var currentUserId = claim.Value;
			var usersWithRoles = await (from user in _context.Users
										join userRole in _context.UserRoles on user.Id equals userRole.UserId
										join role in _context.Roles on userRole.RoleId equals role.Id
										where user.Id != currentUserId
										select new 
										{
											Id = user.Id,
											Email = user.Email,
											FirstName = user.FirstName,
											LastName = user.LastName,
											PhoneNumber = user.PhoneNumber,
											Role = role.Name,
											Locked = user.LockoutEnd == null || user.LockoutEnd < DateTime.UtcNow ? true : false
										}).ToListAsync();

			return Json(new { data = usersWithRoles });


		}


		public async Task<IActionResult> LockUnlock(string? id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null) { return NotFound(); }

			if (user.LockoutEnd == null || user.LockoutEnd < DateTime.UtcNow)
			{
				user.LockoutEnd = DateTime.Now.AddYears(100);
			}
			else
			{
				user.LockoutEnd = DateTime.Now;
			}
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Users", new { area = "Admin" });

		}
	}


}
