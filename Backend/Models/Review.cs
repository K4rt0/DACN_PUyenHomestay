using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("reviews")]
    public class Review : BaseModel
    {
        public byte rating { get; set; } = 0;
        public string? comment { get; set; }

        public User? user { get; set; }
        public Branch? branch { get; set; }
    }
}