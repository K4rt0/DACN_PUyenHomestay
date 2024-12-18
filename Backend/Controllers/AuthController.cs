using System.Diagnostics;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("google")]
        public async Task<ReturnApi> Google([FromQuery] string? access_token)
        {
            int user_id = 0;
            try
            {
                using HttpClient httpClient = new();
                var response = await httpClient.GetAsync("https://oauth2.googleapis.com/tokeninfo?id_token=" + access_token);
                var data = await response.Content.ReadAsStringAsync();
                var googleUser = JsonConvert.DeserializeObject<GoogleUserDto?>(data);
                if (googleUser != null && googleUser.error == null)
                {
                    var user = await _userService.GetByEmailAsync(googleUser.email!);
                    if (user == null)
                    {
                        user = new User
                        {
                            email = googleUser.email,
                            full_name = googleUser.name,
                            password = AuthService.HashPassword("googlelogin"),
                            role = UserRole.Customer
                        };
                        var createUserResult = await _userService.CreateAsync(user);
                        if (createUserResult == null)
                            return new ReturnApi(false, "Đăng nhập thất bại");

                        user_id = createUserResult.id;
                    }
                    else
                        user_id = user.id;

                    return new ReturnApi(true, "Đăng nhập thành công", new
                    {
                        token = Utils.GetJWT("week", 1, new Dictionary<string, object> { { "user_id", user.id.ToString() } })
                    });
                }
                else
                    return new ReturnApi(false, "Đăng nhập thất bại");
            }
            catch (Exception)
            {
                return new ReturnApi(false);
            }
        }
    }
    public class GoogleUserDto
    {
        public object? error { get; set; }
        public string? email { get; set; }
        public string? name { get; set; }
        public string? token { get; set; }
    }
}