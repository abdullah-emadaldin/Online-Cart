using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShop.EFCore.Data;
using MyShop.Model.Enums;
using MyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.EFCore.Initialize
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

       

     
        public void Initialize()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            if (!_roleManager.RoleExistsAsync(Roles.Admin.ToString()).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Editor.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Customer.ToString())).GetAwaiter().GetResult();


                _userManager.CreateAsync(new User { 
                
                    UserName = "Admin@MyShop.com",
                    Email = "Admin@MyShop.com",
                    FirstName = "Admin",
                    LastName = "1",

                
                
                
                },"Admin@123").GetAwaiter().GetResult();

                User admin = _context.Users.FirstOrDefault(x=>x.UserName == "Admin@MyShop.com")!;
                _userManager.AddToRoleAsync(admin, Roles.Admin.ToString()).GetAwaiter().GetResult();


            }


            return;

        }
    }
}
