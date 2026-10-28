using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class CreateModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IVideoService _videoService;

        public CreateModel(DatabaseContext databaseContext, IVideoService videoService)
        {
            _databaseContext = databaseContext;
            _videoService = videoService;
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

            string fileName = await _videoService.UploadVideoAsync(Video!);

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

            return RedirectToPage("/Episode/MyEpisodes");
        }
    }
}
