using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class AllEpisodesModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IVideoService _videoService;

        public AllEpisodesModel(DatabaseContext databaseContext, IVideoService videoService)
        {
            _databaseContext = databaseContext;
            _videoService = videoService;
        }

        public List<Episode> Episodes { get; set; } = new List<Episode>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var episodesQuery = _databaseContext
                .Episode
                .Include(e => e.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                episodesQuery = episodesQuery
                    .Where(e => e.Title.Contains(SearchTerm) ||
                    (e.User != null && e.User.Username.Contains(SearchTerm)));
            }

            Episodes = SortBy switch
            {
                "newest" => await episodesQuery
                    .OrderByDescending(e => e.ReleaseDate)
                    .ToListAsync(),
                "oldest" => await episodesQuery
                    .OrderBy(e => e.ReleaseDate)
                    .ToListAsync(),
                _ => await episodesQuery
                    .OrderByDescending(e => e.ViewerNumber)
                    .ToListAsync()
            };

            return Page();
        }

        public async Task<IActionResult> OnGetPlayAsync(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized();
            }

            var episode = await _databaseContext.Episode.FirstOrDefaultAsync(e => e.EpisodeId == id);

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
