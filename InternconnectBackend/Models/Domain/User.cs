using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models.Domain
{
    public class User
    {
        [Key]
        public required string Username { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public required string Role { get; set; }

    }
}
