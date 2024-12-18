using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("branches")]
    public class Branch : BaseModel
    {
        public bool is_locked { get; set; } = true;
        public string? name { get; set; }
        public string? address { get; set; }
        public string? province { get; set; }
        public string? district { get; set; }
        public string? ward { get; set; }
        public string? description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<BranchContactDetail> branch_contact_details { get; set; } = new HashSet<BranchContactDetail>();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<Room> rooms { get; set; } = new HashSet<Room>();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<User> users { get; set; } = new HashSet<User>();
    }
}