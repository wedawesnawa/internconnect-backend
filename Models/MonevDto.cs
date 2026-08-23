namespace InternconnectBackend.Models
{
    public class MonevDto
    {
        public Guid KodeLogbook { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
    }
}
