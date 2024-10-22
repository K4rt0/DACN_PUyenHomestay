using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet("auth")]
        public ReturnApi AuthorizeAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Xác thực thành công !");
        }

        #region Login
        [HttpPost("login")]
        [ServiceFilter(typeof(RateLimitFilter))]
        public async Task<ReturnApi> LoginAsync([FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (email.Length == 0 || password.Length == 0)
                return new ReturnApi(false, "Tài khoản hoặc mật khẩu không được bỏ trống !");

            User user = await _userService.GetByEmailAsync(email);

            Console.WriteLine(email);
            Console.WriteLine(password);

            if (user == null || user.role != UserRole.Staff)
                return new ReturnApi(false, "Tài khoản hoặc mật khẩu không hợp lệ !");

            if (AuthService.VerifyPassword(user.password, password))
            {
                user.token = Utils.GetJWT(user.id, "year", 1);
                user.password = null;
                return new ReturnApi(true, "Đăng nhập thành công !", user);
            }
            return new ReturnApi(false, "Mật khẩu không chính xác !");
        }
        #endregion
    }
}