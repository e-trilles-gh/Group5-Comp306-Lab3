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

            Episodes = await _databaseContext.Episode
                .Where(e => e.UserId == userId.Value)
                .OrderByDescending(e => e.ReleaseDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var episode = await _databaseContext.Episode.FirstOrDefaultAsync(
                e => e.EpisodeId == id && e.UserId == userId.Value);

            if (episode == null)
            {
                return NotFound();
            }

            _videoService.DeleteVideo(episode.VideoFile);

            _databaseContext.Episode.Remove(episode);

            await _databaseContext.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPlayAsync(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized();
            }

            var episode = await _databaseContext
                .Episode
                .FirstOrDefaultAsync(e =>
                e.EpisodeId == id && e.UserId == userId.Value);

            if (episode == null)
            {
                return NotFound();
            }

            string? filePath = _videoService.GetVideoPath(episode.VideoFile);

            if (filePath == null)
            {
                return NotFound("Video file not found.");
            }

            string contentType = Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".mov" => "video/quicktime",
                _ => "application/octet-stream"
            };

            return PhysicalFile(filePath, contentType);
        }
    }
}
