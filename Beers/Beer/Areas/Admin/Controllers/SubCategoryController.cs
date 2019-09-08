using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beer.Data;
using Beer.Models;
using Beer.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Beer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;
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
        public async Task<IActionResult> Create(SubCategory subCategory)
        {
            if(ModelState.IsValid)
            {
                await _db.SubCategory.AddAsync(subCategory);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            return View(subCategory);
        }
    }
}