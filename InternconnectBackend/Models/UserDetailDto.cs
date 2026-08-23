namespace InternconnectBackend.Models
{
    public class UserDetailDto
    {
        public required string Nama { get; set; }
        public required string Telp { get; set; }
        public required string Bio { get; set; }
        public required string Alamat { get; set; }
        public required string Instansi { get; set; }
        public string? AlamatInstansi { get; set; }
    }
}
