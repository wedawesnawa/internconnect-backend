using System.Text.Json.Serialization;

namespace InternconnectBackend.Models
{
    public class WherebyMeetingResponse
    {
        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public string EndDate { get; set; }

        [JsonPropertyName("roomName")]
        public string RoomName { get; set; }

        [JsonPropertyName("roomUrl")]
        public string RoomUrl { get; set; } // Menggunakan RoomUrl

        [JsonPropertyName("meetingId")]
        public string MeetingId { get; set; }
    }
}
