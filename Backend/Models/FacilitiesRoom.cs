using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("facilities_rooms")]
    public class FacilitiesRoom : BaseModel
    {
        public bool is_bed { get; set; } = false;
        public int quantity { get; set; } = 1;

        public Facility? facility { get; set; }
        public Room? room { get; set; }
    }
}