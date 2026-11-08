using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace group_5_Mohti_Trilles__Lab_3.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }
    }
}
