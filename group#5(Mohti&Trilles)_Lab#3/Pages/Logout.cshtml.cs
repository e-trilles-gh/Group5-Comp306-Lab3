using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace group_5_Mohti_Trilles__Lab_3.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}
