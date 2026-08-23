using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models.Domain
{
    public class LogbookShared
    {
        [Key]
        public int IdShared { get; set; }

        // Foreign Key ke Logbook
        [ForeignKey("Logbook")]
        public Guid KodeLogbook { get; set; }
        public Logbook Logbook { get; set; } = null!;


        // User yang diberikan akses
        [ForeignKey("SharedUser")]
        public string? SharedWith { get; set; }


        public DateTime SharedAt { get; set; }
        public required string Permission { get; set; } // Bisa read-only atau edit

        public List<Monev> Monevs { get; set; } = new();
    }
}
