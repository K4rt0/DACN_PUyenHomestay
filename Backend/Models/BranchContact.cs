using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("branch_contacts")]
    public class BranchContact : BaseModel
    {
        public string? name { get; set; }
        public string? contact_icon { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<BranchContactDetail> branch_contact_details { get; set; } = new HashSet<BranchContactDetail>();
    }
}