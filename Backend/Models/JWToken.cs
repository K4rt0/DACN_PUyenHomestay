namespace Backend.Models
{
    public class JWToken
    {
        public int user_id { get; set; }
        public long iat { get; set; }
        public long exp { get; set; }
    }
}