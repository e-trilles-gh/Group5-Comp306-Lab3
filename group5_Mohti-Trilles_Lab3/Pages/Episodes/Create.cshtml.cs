using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class CreateModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IAmazonS3 _s3Client;

        private const string BucketName = "my-podcast-videos-2026";

        public CreateModel(DatabaseContext databaseContext, IAmazonS3 s3Client)
        {
            _databaseContext = databaseContext;
            _s3Client = s3Client;
        }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public IFormFile? Video { get; set; }

        public void OnGet()
        {
        }

        [RequestSizeLimit(150 * 1024 * 1024)]
        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                ModelState.AddModelError(nameof(Title), "Please enter a title.");
            }

            if (Video == null || Video.Length == 0)
            {
                ModelState.AddModelError(nameof(Video), "Please select a video.");
            }
            else if (Video.Length > 100 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(Video), "The video must be smaller than 100 MB.");
            }
            else
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
                return Page();
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Video!.FileName).ToLowerInvariant();

            using (var stream = Video.OpenReadStream())
            {
                var request = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = fileName,
                    InputStream = stream
                };

                await _s3Client.PutObjectAsync(request);
            }

            Episode episode = new Episode
            {
                Title = Title,
                VideoFile = fileName,
                UserId = userId.Value,
                ViewerNumber = 0,
                ReleaseDate = DateTime.Now
            };

            _databaseContext.Episode.Add(episode);

            await _databaseContext.SaveChangesAsync();

            return RedirectToPage("/Episodes/MyEpisodes");
        }
    }
}