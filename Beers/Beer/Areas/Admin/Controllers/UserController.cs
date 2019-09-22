using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Beer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Beer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Find list of users registered
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity) this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var usersList = await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync();

            return View(usersList);
        }
    }
}