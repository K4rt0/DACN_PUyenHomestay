using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("rooms")]
    public class Room : BaseModel
    {
        public int room_number { get; set; }
        public decimal cost { get; set; }
        public int room_width { get; set; }
        public Branch? branch { get; set; }

        public ICollection<RoomImage> room_images { get; set; } = new HashSet<RoomImage>();
        public ICollection<FacilitiesRoom> facilities_rooms { get; set; } = new HashSet<FacilitiesRoom>();
    }
}