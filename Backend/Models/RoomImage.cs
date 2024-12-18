using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("room_images")]
    public class RoomImage : BaseModel
    {
        public string? url { get; set; }
        public string? type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Room? room { get; set; }
    }
}