using group_5_Mohti_Trilles__Lab_3.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace group_5_Mohti_Trilles__Lab_3.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly PasswordHasher<Model.User> _passwordHasher = new();

        public RegistrationModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [BindProperty]
        public Model.User User { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string RetypePassword { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                bool userExists = await _databaseContext.User.AnyAsync(u =>
                            u.Username == User.Username || u.Email == User.Email);

                if (userExists)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Username or email is already registered.");
                    return Page();
                }
                User.PasswordHash = _passwordHasher.HashPassword(User, Password);

                _databaseContext.User.Add(User);
                await _databaseContext.SaveChangesAsync();

                TempData["success"] = "Uesr registered successfully.";

                return RedirectToPage("/Login");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error registration.";
                return Page();
            }

            return RedirectToPage();
        }
    }
}
