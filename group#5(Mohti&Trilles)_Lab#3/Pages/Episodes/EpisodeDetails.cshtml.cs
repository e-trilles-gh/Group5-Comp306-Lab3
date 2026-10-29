using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace group_5_Mohti_Trilles__Lab_3.Pages.Episodes
{
    public class EpisodeDetailsModel : PageModel
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IVideoService _videoService;

        public EpisodeDetailsModel(DatabaseContext databaseContext, IVideoService videoService)
        {
            _databaseContext = databaseContext;
            _videoService = videoService;
        }

        public Episode Episode { get; set; }

        public List<Comment> Comments { get; set; }

        public int CurrentUserId { get; set; }

        public int? EditingCommentId { get; set; }

        public string? SuccessMessage { get; set; }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        public async Task<IActionResult> OnGetAsync(int id, int? editCommentId)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            CurrentUserId = userId.Value;
            EditingCommentId = editCommentId;

            var episode = await _databaseContext.Episode
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EpisodeId == id);

            if(episode == null)
            {
                return NotFound();
            }

            Episode = episode;

            Comments = await _databaseContext.Comment
                .Where(c => c.EpisodeId == id)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

            if (EditingCommentId.HasValue)
            {
                var comment = Comments.FirstOrDefault(
                    c => c.CommentId == EditingCommentId.Value);

                if (comment == null || comment.UserId != CurrentUserId || !CanModifyComment(comment))
                {
                    EditingCommentId = null;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int id, string commentText)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            bool episodeExists = await _databaseContext.Episode
                .AnyAsync(e => e.EpisodeId == id);

            if (!episodeExists)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";

                return RedirectToPage(new { id });
            }

            var comment = new Comment
            {
                CommentText = commentText.Trim(),
                EpisodeId = id,
                UserId = userId.Value,
                CreateDate = DateTime.Now
            };

            _databaseContext.Comment.Add(comment);

            await _databaseContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your comment has been added.";

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostEditCommentAsync(int id, int commentId, string commentText)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var comment = await _databaseContext.Comment
                .FirstOrDefaultAsync(c =>
                c.CommentId == commentId && c.EpisodeId == id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != userId.Value || !CanModifyComment(comment))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";

                return RedirectToPage(new { id, editCommentId = commentId });
            }

            comment.CommentText = commentText.Trim();

            await _databaseContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your comment has been updated.";

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int id, int commentId)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var comment = await _databaseContext.Comment
                .FirstOrDefaultAsync(c => c.CommentId == commentId &&
                c.EpisodeId == id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != userId.Value || !CanModifyComment(comment))
            {
                return Forbid();
            }

            _databaseContext.Comment.Remove(comment);

            await _databaseContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your comment has been deleted.";

            return RedirectToPage(new { id });
        }

        private static bool CanModifyComment(Comment comment)
        {
            DateTime now = DateTime.Now;

            return comment.CreateDate <= now && now < comment.CreateDate.AddHours(24);
        }

        public async Task<IActionResult> OnGetPlayAsync(int id)
        {
            int? userId = GetCurrentUserId();

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

            var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete
            );

            return new FileStreamResult(stream, contentType)
            {
                EnableRangeProcessing = true
            };
        }
    }
}
