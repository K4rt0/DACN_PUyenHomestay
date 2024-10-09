using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Models.Enums;

namespace Backend.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [MaxLength(255)]
        public string? full_name { get; set; }

        [MaxLength(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? password { get; set; }

        [MaxLength(15)]
        public string? phone_number { get; set; }

        [MaxLength(100)]
        public string? email { get; set; }
        public DateOnly? birthday { get; set; }
        
        [MaxLength(20)]
        public string? identifier { get; set; }
        public int reward_points { get; set; } = -1;
        public bool is_verified { get; set; } = false;
        public bool is_locked { get; set; } = false;
        public UserRole role { get; set; } = UserRole.Customer;
        public Branch? branch { get; set; }

        public ICollection<Reservation> reservations { get; set; } = new HashSet<Reservation>();
        public ICollection<Review> reviews { get; set; } = new HashSet<Review>();

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? token { get; set; }
    }
}