﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Beer.Models;
using Beer.Data;
using Beer.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Beer.Utility;

namespace Beer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var cnt = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value)
                        .ToList().Count;

                HttpContext.Session.SetInt32(SD.cntShoppingCart, cnt);
            }


            return View(IndexVM);
        }



        
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart() {
                   MenuItem = menuItemFromDb,
                   MenuItemId = menuItemFromDb.Id
            };

            return View(cartObj);

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart obj)
        {
            obj.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity) this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                obj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart
                    .Where(c => c.ApplicationUserId == obj.ApplicationUserId && c.MenuItemId == obj.MenuItemId)
                    .FirstOrDefaultAsync();

                if(cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(obj);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + obj.Count;
                }

                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart
                    .Where(c => c.ApplicationUserId == obj.ApplicationUserId)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.cntShoppingCart, count);

                return RedirectToAction("Index");


            }
            else
            {
                var menuItemFromDb = await _db.MenuItem.Include(m => m.Category)
                    .Include(m => m.SubCategory)
                    .Where(m => m.Id == obj.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart cartObj = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(cartObj);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
