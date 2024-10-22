using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("facilities_rooms")]
    public class FacilitiesRoom : BaseModel
    {
        public bool is_bed { get; set; } = false;
        public int quantity { get; set; } = 1;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Facility? facility { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Room? room { get; set; }
    }
}