using System.ComponentModel.DataAnnotations;

namespace group_5_Mohti_Trilles__Lab_3.Model
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
