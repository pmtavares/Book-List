using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beer.Data;
using Beer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Beer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get Category list
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Category.ToListAsync();
            return View(categories);
        }

        //GET for create
        public IActionResult Create()
        {
            return View();
        }

        //POST for Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //GET Edit

        public async Task<IActionResult> Edit(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }

            var category = await _db.Category.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);

        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            Category category = _db.Category.Find(id);
            return View(category);
        }
    }
}