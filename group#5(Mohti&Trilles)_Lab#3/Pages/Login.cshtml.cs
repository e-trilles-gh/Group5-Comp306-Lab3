using group_5_Mohti_Trilles__Lab_3.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace group_5_Mohti_Trilles__Lab_3.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly PasswordHasher<User> _passwordHasher = new();
        
        public LoginModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [BindProperty]
        [Required]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _databaseContext.User.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");

                return Page();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");

                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToPage("/Index");
        }
    }
}
