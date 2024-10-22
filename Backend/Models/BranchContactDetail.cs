using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("branch_contact_details")]
    public class BranchContactDetail : BaseModel
    {
        public string? value { get; set; }
        public bool is_locked { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Branch? branch { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public BranchContact? branch_contact { get; set; }
    }
}