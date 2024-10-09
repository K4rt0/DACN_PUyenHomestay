using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("facilities")]
    public class Facility : BaseModel
    {
        public string? icon { get; set; }
        public string? name { get; set; }
        public ICollection<FacilitiesRoom> facilities_rooms { get; set; } = new HashSet<FacilitiesRoom>();
    }
}