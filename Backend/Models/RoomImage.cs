using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("room_images")]
    public class RoomImage : BaseModel
    {
        public string? url { get; set; }
        public Room? room { get; set; }
    }
}