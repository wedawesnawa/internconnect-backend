using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models
{
    public class VerifStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
