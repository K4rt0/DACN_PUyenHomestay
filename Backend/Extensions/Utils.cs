using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Backend.Models;
using DotEnv.Core;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Backend.Extensions
{
    public class Utils
    {
        public static string DB_MYSQL
        {
            get
            {
                try
                {
                    return "Server=" + EnvReader.Instance["DB_MYSQL_HOST"]
                    + ";User=" + EnvReader.Instance["DB_MYSQL_USER"]
                    + ";Password=" + EnvReader.Instance["DB_MYSQL_PASS"] + ";Database=" + EnvReader.Instance["DB_MYSQL_NAME"] + ";Port=3306";
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(EnvReader.Instance["DB_MYSQL_USER"]);
                    Console.WriteLine("Can't get DB_MYSQL!");
                    Console.ResetColor();
                }
                return string.Empty;
            }
        }

        #region JWT
        public static string SALT = EnvReader.Instance["SALT"];
        private static string JWT_HEADER => "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";

        public static string Base64encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Base64encode(plainTextBytes);
        }

        public static string Base64encode(byte[] plainTextBytes)
        {
            return Convert.ToBase64String(plainTextBytes).Split('=')[0].Replace('+', '-').Replace('/', '_');
        }

        public static string Base64decode(string encodedString)
        {
            string s = encodedString;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new Exception("Illegal base64url string!");
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }

        private static string HmacSha256(string data, string secret)
        {
            return Base64encode(new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        public static string GetJWT(int id, string time, int time_set)
        {
            JWToken payload = new();
            DateTimeOffset now = DateTimeOffset.Now;
            payload.user_id = id;
            payload.iat = now.ToUnixTimeSeconds();
            switch (time)
            {
                case "minute":
                    payload.exp = now.AddMinutes(time_set).ToUnixTimeSeconds();
                    break;
                case "hour":
                    payload.exp = now.AddHours(time_set).ToUnixTimeSeconds();
                    break;
                case "day":
                    payload.exp = now.AddDays(time_set).ToUnixTimeSeconds();
                    break;
                case "week":
                    payload.exp = now.AddDays(7 * time_set).ToUnixTimeSeconds();
                    break;
                case "month":
                    payload.exp = now.AddMonths(time_set).ToUnixTimeSeconds();
                    break;
                case "year":
                    payload.exp = now.AddYears(time_set).ToUnixTimeSeconds();
                    break;
                default:
                    payload.exp = now.AddDays(7).ToUnixTimeSeconds();
                    break;
            }
            string Header_Payload = Base64encode(JWT_HEADER) + "." + Base64encode(JsonConvert.SerializeObject(payload));
            string signature = HmacSha256(Header_Payload, EnvReader.Instance["SALT"]);
            return Header_Payload + "." + signature;
        }

        public static JWToken? ParseJWT(string token)
        {
            try
            {
                string[] parts = token.Split('.');
                JWToken? payload = JsonConvert.DeserializeObject<JWToken>(Base64decode(parts[1]));
                string signature = parts[2];
                if (payload != null)
                {
                    if (payload.exp > 0 && payload.exp < DateTimeOffset.Now.ToUnixTimeSeconds()) return null;

                    string headerAndPayloadHashed = HmacSha256(parts[0] + "." + parts[1], EnvReader.Instance["SALT"]);
                    if (headerAndPayloadHashed == signature) return payload;
                }
                return null;
            }
            catch { return null; }
        }

        public static string? AuthorizationJWT(StringValues headerValue)
        {
            if (headerValue.Count > 0)
            {
                string[] values = headerValue[0]!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (values.Length == 2)
                    return values[1];
            }
            return null;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            string pattern = @"^(\+84|0)\d{9}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(phoneNumber);
        }

        public static bool IsValidVietnamIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return false;

            string pattern = @"^\d{9}$|^\d{12}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(identifier);
        }
        #endregion
    }
}