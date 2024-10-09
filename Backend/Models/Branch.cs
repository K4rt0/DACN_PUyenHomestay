using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("branches")]
    public class Branch : BaseModel
    {
        public string? name { get; set; }
        public string? address { get; set; }
        public string? province { get; set; }
        public string? district { get; set; }
        public string? ward { get; set; }
        public string? description { get; set; }

        public ICollection<BranchContactDetail> branch_contact_details { get; set; } = new HashSet<BranchContactDetail>();
        public ICollection<Room> rooms { get; set; } = new HashSet<Room>();
        public ICollection<Review> reviews { get; set; } = new HashSet<Review>();
        public ICollection<User> users { get; set; } = new HashSet<User>();
    }
}