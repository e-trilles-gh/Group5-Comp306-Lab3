using System.ComponentModel.DataAnnotations;

namespace group_5_Mohti_Trilles__Lab_3.Model
{
    public class Episode
    {
        public int EpisodeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string VideoFile { get; set; }

        public int UserId { get; set; }

        public int ViewerNumber { get; set; }

        public DateTime ReleaseDate { get; set; }

        public User? User { get; set; }
    }
}
