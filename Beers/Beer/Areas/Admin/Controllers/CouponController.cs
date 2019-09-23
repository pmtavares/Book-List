using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Beer.Data;
using Beer.Models;
using Beer.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Beer.Areas.Admin.Controllers
{


    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class CouponController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
            
        }
        public async Task<IActionResult> Index()
        {
            List<Coupon> coupons = await _db.Coupon.ToListAsync();
            return View(coupons);
        }

        public IActionResult  Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Coupon coupon)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if(files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupon.Picture = p1;
                }
                _db.Coupon.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);            

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                Coupon couponFromDb = await _db.Coupon.FindAsync(coupon.Id);

                if(couponFromDb == null)
                {
                    return NotFound();
                }


                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDb.Picture = p1;
                }
                couponFromDb.Name = coupon.Name;
                couponFromDb.CouponType = coupon.CouponType;
                couponFromDb.Discount = coupon.Discount;
                couponFromDb.MinimumAmount = coupon.MinimumAmount;
                couponFromDb.IsActive = coupon.IsActive;

                _db.Update(couponFromDb);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            Coupon coupon = await _db.Coupon.FindAsync(id);
            return View(coupon);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == 0)
            {
                return NotFound();
            }

            var model = await _db.Coupon.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var model = await _db.Coupon.FindAsync(id);

            if (model == null)
            {
                return View();
            }

            _db.Remove(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}