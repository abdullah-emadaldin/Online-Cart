using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.EFCore.Data;
using MyShop.Model.Interfaces;
using MyShop.Model.Models;

namespace MyShop.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
	
		public IActionResult Index()
        {
            var categories =  _unitOfWork.Category.GetAll();

            return View(categories);
        }
	
		[HttpGet]
        public IActionResult Create()
        {
            return View();
        }

		[Area("Admin")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            await _unitOfWork.Category.Add(category);
            await _unitOfWork.SaveChangesAsync();
            TempData["Create"] = $"Category {category.Name} Has Been Added Successfully";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || id == 0) { NotFound(); }
            var category = await _unitOfWork.Category.Get(id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.Update(category);

                TempData["Update"] = $"Category {category.Name} Has Been Update Successfully";
            }
            return RedirectToAction("Index");

        }



        //[HttpGet]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //	if (id == null || id == 0)
        //	{
        //		return NotFound();
        //	}

        //	var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        //	return View(category);

        //}


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.Category.Remove(id);
            await _unitOfWork.SaveChangesAsync();
            TempData["Delete"] = $"Category Has Been Update Successfully";
            return RedirectToAction("Index");

        }
    }
}
