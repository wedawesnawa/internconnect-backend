using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models.Domain
{
    public class UserDetail
    {
        [Key]
        public int UserId { get; set; }

        public string? Nama { get; set; }
        public string? Telp { get; set; }
        public string? Bio { get; set; }
        public string? Alamat { get; set; }
        public string? Instansi { get; set; }
        public string? AlamatInstansi { get; set; }
        public string? profileUrl { get; set; }

        public string? FileUrl { get; set; }

        public required string Username { get; set; }

    }
}
