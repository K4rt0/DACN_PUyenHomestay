using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("branch_contact_details")]
    public class BranchContactDetail : BaseModel
    {
        public string? value { get; set; }
        public string? description { get; set; }
        public bool is_locked { get; set; } = false;
        public Branch? branch { get; set; }
        public BranchContact? branch_contact { get; set; }
    }
}