using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Models.Enums;

namespace Backend.Models
{
    [Table("reservations")]
    public class Reservation
    {
        [Key]
        public Guid booking_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? created_at { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? updated_at { get; set; }
        public DateOnly? check_in { get; set; }
        public DateOnly? check_out { get; set; }
        public int guest_amount { get; set; } = 0;
        public double total_price { get; set; } = 0;
        public ReservationStatus status { get; set; } = ReservationStatus.Pending;
        public PaymentMethod payment_method { get; set; } = PaymentMethod.PayInHotel;
        public string? comment { get; set; }
        public int? rating { get; set; }
        public bool payment_status { get; set; } = false;
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public int reward_points { get; set; } = 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Room? room { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public User? user { get; set; }
    }
}