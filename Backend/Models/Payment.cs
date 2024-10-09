using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Enums;

namespace Backend.Models
{
    [Table("payments")]
    public class Payment : BaseModel
    {
        public PaymentMethod payment_method { get; set; } = PaymentMethod.Cash;
        public User? user { get; set; }
        public Reservation? reservation { get; set; }
    }
}