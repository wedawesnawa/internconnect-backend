using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models
{
    public class UpdateUserRoleDto
    {

        [Required]
        public required string NewRole { get; set; }

        public IFormFile? File { get; set; }

        [Required]
        public required string Instansi { get; set; }
    }
}
