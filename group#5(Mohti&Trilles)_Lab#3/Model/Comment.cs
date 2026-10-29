using System.ComponentModel.DataAnnotations;

namespace group_5_Mohti_Trilles__Lab_3.Model
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public string CommentText { get; set; }

        public int EpisodeId { get; set; }

        public DateTime CreateDate { get; set; }

        public int UserId { get; set; }

        public Episode? Episode { get; set; }

        public User? User { get; set; }
    }
}
