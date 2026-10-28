using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class MyEpisodesModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IVideoService _videoService;

        public MyEpisodesModel(DatabaseContext databaseContext, IVideoService videoService)
        {
            _databaseContext = databaseContext;
            _videoService = videoService;
        }

        public List<Episode> Episodes { get; set; } = new List<Episode>();

        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            Episodes = await _databaseContext.Episode.Where(e => e.UserId == userId.Value)
                .OrderByDescending(e => e.ReleaseDate).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var episode = _databaseContext.Episode.Find(id);

            if (episode == null)
            {
                return NotFound();
            }

            _databaseContext.Episode.Remove(episode);
            _databaseContext.SaveChanges();
            TempData["success"] = "Deleted successfully";

            return RedirectToPage();
        }
    }
}
