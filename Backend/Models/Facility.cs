using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("facilities")]
    public class Facility : BaseModel
    {
        public string? icon { get; set; }
        public string? name { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<FacilitiesRoom> facilities_rooms { get; set; } = new HashSet<FacilitiesRoom>();
    }
}