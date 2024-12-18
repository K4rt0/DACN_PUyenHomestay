namespace Backend.Models
{
    public class JWToken
    {
        public bool is_expired { get; set; } = false;
        public long iat { get; set; }
        public long exp { get; set; }
        public Dictionary<string, object>? custom_data { get; set; }
    }
}