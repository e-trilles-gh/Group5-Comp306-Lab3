using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Amazon.S3;
using Amazon.S3.Model;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class EditEpisodeModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IAmazonS3 _s3Client;

        private const string BucketName = "my-podcast-videos-2026";

        public EditEpisodeModel(DatabaseContext databaseContext, IAmazonS3 s3Client)
        {
            _databaseContext = databaseContext;
            _s3Client = s3Client;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public int ViewerNumber { get; set; }

        [BindProperty]
        public IFormFile? Video {  get; set; }

        public string CurrentVideoFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var episode = await _databaseContext.Episode.FirstOrDefaultAsync(e =>
            e.EpisodeId == id && e.UserId == userId.Value);

            if (episode == null)
            {
                return NotFound();
            }

            Id = episode.EpisodeId;
            Title = episode.Title;
            ViewerNumber = episode.ViewerNumber;
            CurrentVideoFile = episode.VideoFile;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var episode = await _databaseContext.Episode.FirstOrDefaultAsync(
                e => e.EpisodeId == Id && e.UserId == userId.Value);

            if (episode == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                ModelState.AddModelError(nameof(Title), "Please enter a title.");
            }

            if (ViewerNumber < 0)
            {
                ModelState.AddModelError(nameof(ViewerNumber), "The viewer number cannot be less than zero.");
            }

            if (Video != null && Video.Length > 0)
            {
                string extension = Path.GetExtension(Video.FileName).ToLowerInvariant();

                string[] allowedExtensions = { ".mp4", ".mov", ".webm" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(Video), "Only MP4, MOV, and WEBM videos are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                CurrentVideoFile = episode.VideoFile;
                return Page();
            }

            string oldVideoFile = episode.VideoFile;

            if (Video != null && Video.Length > 0)
            {
                string newVideoFile = Guid.NewGuid().ToString() + Path.GetExtension(Video.FileName).ToLowerInvariant();

                //uploads the new video to s3
                using (var stream = Video.OpenReadStream())
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = BucketName,
                        Key = newVideoFile,
                        InputStream = stream,
                        ContentType = Video.ContentType
                    };

                    await _s3Client.PutObjectAsync(request);
                }

                episode.VideoFile = newVideoFile;
            }

            episode.Title = Title;
            episode.ViewerNumber = ViewerNumber;

            await _databaseContext.SaveChangesAsync();

            //Deletes the old video only after the database is updated
            if (Video != null && Video.Length > 0)
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = oldVideoFile
                });
            }

            return RedirectToPage("/Episodes/MyEpisode");
        }
    }
}
