using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Amazon.S3;
using Amazon.S3.Model;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class MyEpisodesModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IAmazonS3 _s3Client;

        private const string BucketName = "my-podcast-videos-2026";

        public MyEpisodesModel(DatabaseContext databaseContext, IAmazonS3 s3Client)
        {
            _databaseContext = databaseContext;
            _s3Client = s3Client;
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

            try
            {
                string videoFile = Path.GetFileName(episode.VideoFile);

                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = videoFile
                });

                _databaseContext.Episode.Remove(episode);
                await _databaseContext.SaveChangesAsync();

                return RedirectToPage();
            }
            catch (AmazonS3Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Unable to delete the video file from S3. {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Unable to delete episode from database. {ex.Message}");
            }

            Episodes = await _databaseContext.Episode
                    .Where(e => e.UserId == userId.Value)
                    .OrderByDescending(e => e.ReleaseDate)
                    .ToListAsync();

            return Page();
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
