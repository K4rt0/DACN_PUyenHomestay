using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("rooms")]
    public class Room : BaseModel
    {
        public string? room_name { get; set; }
        public string? room_number { get; set; }
        public decimal cost { get; set; }
        public int room_width { get; set; }
        public bool is_locked { get; set; } = false;
        public int room_max_adults { get; set; } = 2;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Branch? branch { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<RoomImage> room_images { get; set; } = new HashSet<RoomImage>();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<FacilitiesRoom> facilities_rooms { get; set; } = new HashSet<FacilitiesRoom>();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<Reservation> reservations { get; set; } = new HashSet<Reservation>();
    }
}