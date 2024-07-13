using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyShop.EFCore.Data;
using MyShop.Model.Interfaces;
using MyShop.Model.Models;
using MyShop.Model.ViewModels;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MyShop.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;  
        }
	
		public  IActionResult Index()
        {
            var products =  _unitOfWork.Product.GetAll();
            
            return View(products);
        }

		public async Task<IActionResult> GetData()
		{
			//var products = _unitOfWork.Product.GetAll().Select(x=> new { x.Name , x.Description , x.Price , category = x.Category.Name});
			var products = await _unitOfWork.Product.GetAllAsync();


			var dataa = products.Select(p => new
			{
				name = p.Name,
				description = p.Description,
				price = p.Price ,
				category = p.Category != null ? p.Category.Name : null ,// Handle null Category
                id=p.Id
			});
			return Json(new { data = dataa });
		}

		[HttpGet]
        public IActionResult Create()
        {
            
            ProductVM productVM=new ProductVM() 
            { 
                Product=new Product(),
                CategoryList =_unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text=x.Name,
                    Value = x.Id.ToString()
                })
            
            };

            return View(productVM);
        }

		[Area("Admin")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM ProductVM,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);
                    using(var filestream = new FileStream(Path.Combine(upload,filename+ext),FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                    }
                    ProductVM.Product.Img = @"Images\Products\" + filename + ext;

                }
            }



            await _unitOfWork.Product.Add(ProductVM.Product);
            await _unitOfWork.SaveChangesAsync();
            TempData["Create"] = $"Product {ProductVM.Product.Name} Has Been Added Successfully";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || id == 0) { NotFound(); }
            ProductVM productVM = new ProductVM()
            {
                
               
                 Product = await _unitOfWork.Product.Get(id),
                
				CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				})

			};
			
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM ProductVM,IFormFile? file)
        {
            if (ModelState.IsValid)
            {


                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    if (ProductVM.Product.Img != null)
                    {
                        var oldimg=Path.Combine(RootPath, ProductVM.Product.Img.TrimStart('\\'));
                        if(System.IO.File.Exists(oldimg))
                            System.IO.File.Delete(oldimg);

                    }

                    using (var filestream = new FileStream(Path.Combine(upload, filename + ext), FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                    }
                    ProductVM.Product.Img = @"Images\Products\" + filename + ext;

                }


                await _unitOfWork.Product.Update(ProductVM.Product);

                TempData["Update"] = $"Product {ProductVM.Product.Name} Has Been Update Successfully";
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

        //	var Product = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        //	return View(Product);

        //}


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            
            var product = await _unitOfWork.Product.Get(id);
            if (product == null) { return Json(new { success = false, message = "Error Happened While Deleting" }); }
            await _unitOfWork.Product.Remove(id);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, product.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
                System.IO.File.Delete(oldimg);
            var name = product.Name;
            await _unitOfWork.SaveChangesAsync();
            TempData["Delete"] = $"Product Has Been Deleted Successfully";
            return Json(new { success = true, message = $"Product {name} Has Been Deleted" });

        }
    }
}
