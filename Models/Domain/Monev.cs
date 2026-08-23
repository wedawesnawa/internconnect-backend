using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models.Domain
{
    public class Monev
    {
        [Key]
        public int IdMonev { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string? RoomUrl { get; set; }

        public int IdShared { get; set; }

        // Foreign Key ke Logbook
        [ForeignKey("Logbook")]
        public Guid KodeLogbook { get; set; }
        public virtual Logbook? Logbook { get; set; }

        [ForeignKey("IdShared")]
        public LogbookShared? LogbookShared { get; set; }

    }
}
