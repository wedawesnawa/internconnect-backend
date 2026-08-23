namespace InternconnectBackend.Models
{
    public class SharedDto
    {
        public int IdShared { get; set; }
        public string? SharedWith { get; set; }
        public string Permission { get; set; } = "read-only";
    }
}
