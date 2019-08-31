using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListRazor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookListRazor.Pages.BookList
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string Message { get; set; }
        public CreateModel( ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Book Book { get; set; }
        //Page load initialy
        public void OnGet()
        {

        }

        public async Task<ActionResult> OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            _db.Book.Add(Book);

            await _db.SaveChangesAsync();
            Message = "Data created successfully";
            return RedirectToPage("Index");
        }
    }
}