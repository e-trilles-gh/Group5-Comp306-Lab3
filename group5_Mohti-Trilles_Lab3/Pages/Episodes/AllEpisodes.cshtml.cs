using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Amazon.S3;
using Amazon.S3.Model;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class AllEpisodesModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IAmazonS3 _s3Client;

        private const string BucketName = "my-podcast-videos-2026";

        public AllEpisodesModel(DatabaseContext databaseContext, IAmazonS3 s3Client)
        {
            _databaseContext = databaseContext;
            _s3Client = s3Client;
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

            string videoFile = Path.GetFileName(episode.VideoFile);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = videoFile,
                Expires = DateTime.Now.AddMinutes(30),
                Verb = HttpVerb.GET
            };

            string videoUrl = _s3Client.GetPreSignedURL(request);

            return Redirect(videoUrl);
        }
    }
}
