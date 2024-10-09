using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("branch_contacts")]
    public class BranchContact : BaseModel
    {
        public string? name { get; set; }
        public string? contact_icon { get; set; }

        public ICollection<BranchContactDetail> branch_contact_details { get; set; } = new HashSet<BranchContactDetail>();
    }
}