using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.VietnamProvinces
{
    [Table("districts")]
    public class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string? name { get; set; }
        public string? name_en { get; set; }
        public string? full_name { get; set; }
        public string? full_name_en { get; set; }
        public string? code_name { get; set; }
        public Province? province { get; set; }
        public ICollection<Ward> wards { get; set; } = new HashSet<Ward>();
    }
}