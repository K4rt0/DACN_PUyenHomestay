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
        public string? password { get; set; }

        [MaxLength(15)]
        public string? phone_number { get; set; }

        [MaxLength(100)]
        public string? email { get; set; }

        public int reward_points { get; set; } = 0;
        public bool is_verified { get; set; } = false;
        public bool is_locked { get; set; } = false;
        public UserRole role { get; set; } = UserRole.Customer;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Branch? branch { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<Reservation> reservations { get; set; } = new HashSet<Reservation>();

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? token { get; set; }
    }
}