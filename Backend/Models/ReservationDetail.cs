using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("reservation_details")]
    public class ReservationDetail : BaseModel
    {
        public int quantity { get; set; } = 1;
        public decimal price { get; set; } = 0;
        public Reservation? reservation { get; set; }
        public Room? room { get; set; }
    }
}