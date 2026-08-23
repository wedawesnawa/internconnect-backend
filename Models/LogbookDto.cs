using InternconnectBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models
{
    public class LogbookDto
    {
        [Required]
        public required string Content { get; set; }

        [Required]
        public required DateTime DateStart { get; set; }

        [Required]
        public required DateTime DateEnd { get; set; }

        [Required]
        public required string Status { get; set; }

        [Required]
        public required string Deskripsi { get; set; }

        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [MaxFileSize(10 * 1024 * 1024)] // Maksimal 10MB
        public IFormFile? Image { get; set; }
    }
}
