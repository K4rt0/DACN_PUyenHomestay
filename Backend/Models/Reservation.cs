using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Enums;

namespace Backend.Models
{
    [Table("reservations")]
    public class Reservation : BaseModel
    {
        public DateTime? check_in { get; set; }
        public DateTime? check_out { get; set; }
        public int guest_amount { get; set; } = 0;
        public decimal total_price { get; set; } = 0;
        public ReservationStatus status { get; set; } = ReservationStatus.Pending;
        public string? reason_cancel { get; set; }
        public PaymentMethod payment_method { get; set; } = PaymentMethod.Cash;
        
        public User? user { get; set; }

        public ICollection<ReservationDetail> reservation_details { get; set; } = new HashSet<ReservationDetail>();
    }
}