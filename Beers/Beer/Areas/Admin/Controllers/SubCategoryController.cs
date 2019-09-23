using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beer.Data;
using Beer.Models;
using Beer.Models.ViewModels;
using Beer.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Beer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class SubCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();

            return View(subCategories);
        }

        public async Task<IActionResult> Create()
        {
            CategorySubCategoryViewModel model = new CategorySubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategorySubCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCatExists = _db.SubCategory.Include(p => p.Category)
                    .Where(s=> s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);
                if(doesSubCatExists.Count() > 0)
                {
                    StatusMessage = "Error: Sub Category exists under " + doesSubCatExists.First().Category.Name + ". Use another name.";
                }
                else
                {

                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }

            CategorySubCategoryViewModel modelVM = new CategorySubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            CategorySubCategoryViewModel model = new CategorySubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategorySubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCatExists = _db.SubCategory.Include(p => p.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

                if (doesSubCatExists.Count() > 0)
                {
                    StatusMessage = "Error: Sub Category exists under " + doesSubCatExists.First().Category.Name + ". Use another name.";
                }
                else
                {
                    var subCat = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCat.Name = model.SubCategory.Name;

                    
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }

            CategorySubCategoryViewModel modelVM = new CategorySubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            CategorySubCategoryViewModel model = new CategorySubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            return View(model);

        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == 0)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            CategorySubCategoryViewModel model = new CategorySubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            _db.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}