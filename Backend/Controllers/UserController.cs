using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Common Methods Class
        private async Task<User?> GetCurrentUserAsync()
        {
            if (HttpContext.Items.TryGetValue("user_id", out object? userId) &&
                int.TryParse(userId?.ToString(), out int id))
            {
                return await _userService.GetByIdAsync(id);
            }
            return null;
        }

        private ReturnApi ValidateUser(User user, bool isUpdate = false)
        {
            if (!isUpdate && string.IsNullOrEmpty(user.password))
                return new ReturnApi(false, "Mật khẩu không được bỏ trống !");

            if (!isUpdate && !Utils.IsValidEmail(user.email!.Trim()))
                return new ReturnApi(false, "Email không hợp lệ !");

            if (string.IsNullOrEmpty(user.full_name))
                return new ReturnApi(false, "Họ tên không được bỏ trống !");

            if (user.birthday > DateOnly.FromDateTime(DateTime.Now))
                return new ReturnApi(false, "Ngày sinh không hợp lệ !");

            if (!string.IsNullOrEmpty(user.phone_number) && !Utils.IsValidPhoneNumber(user.phone_number.Trim()))
                return new ReturnApi(false, "Số điện thoại không hợp lệ !");

            if (!string.IsNullOrEmpty(user.identifier) && !Utils.IsValidVietnamIdentifier(user.identifier.Trim()))
                return new ReturnApi(false, "CMND/CCCD không hợp lệ !");

            return new ReturnApi(true);
        }
        #endregion

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet]
        public async Task<ReturnApi> GetAllAsync()
        {
            return new ReturnApi(true, "Lấy danh sách tài khoản thành công !", await _userService.GetAllAsync());
        }

        [JwtAuthorize]
        [HttpGet("profile")]
        public async Task<ReturnApi> GetByIdAsync()
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            user.password = null;
            return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", user);

        }

        [JwtAuthorize]
        [HttpPut]
        public async Task<ReturnApi> UpdateAsync([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? existingUser = await GetCurrentUserAsync();
            if (existingUser == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            ReturnApi validationResult = ValidateUser(user, true);
            if (!validationResult.success)
                return validationResult;

            // Update fields
            if (user.birthday != null) existingUser.birthday = user.birthday;
            if (!string.IsNullOrEmpty(user.email)) existingUser.email = user.email;
            if (!string.IsNullOrEmpty(user.full_name)) existingUser.full_name = user.full_name;
            if (!string.IsNullOrEmpty(user.phone_number)) existingUser.phone_number = user.phone_number;
            if (!string.IsNullOrEmpty(user.identifier)) existingUser.identifier = user.identifier;

            existingUser = await _userService.UpdateAsync(existingUser);

            if (existingUser == null)
                return new ReturnApi(false, "Đã có lỗi xảy ra trong quá trình cập nhật thông tin tài khoản !");

            return new ReturnApi(true, "Cập nhật thông tin tài khoản thành công !");
        }

        [JwtAuthorize]
        [HttpPut("change-password")]
        public async Task<ReturnApi> UpdatePasswordAsync([FromForm] string old_password, [FromForm] string new_password)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (new_password.Length == 0)
                return new ReturnApi(false, "Mật khẩu không được bỏ trống !");

            if (old_password.Equals(new_password))
                return new ReturnApi(false, "Mật khẩu mới không được trùng với mật khẩu cũ !");

            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false);

            if (!AuthService.VerifyPassword(user.password, old_password))
                return new ReturnApi(false, "Mật khẩu cũ không chính xác !");

            user.password = AuthService.HashPassword(new_password);

            user = await _userService.UpdateAsync(user);
            if (user == null)
                return new ReturnApi(false, "Đã có lỗi xảy ra trong quá trình cập nhật mật khẩu !");

            return new ReturnApi(true, "Cập nhật mật khẩu thành công !");
        }

        [JwtAuthorize]
        [HttpDelete]
        public async Task<ReturnApi> DeleteUserAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false);

            await _userService.DeleteAsync(user);

            return new ReturnApi(true, "Xoá tài khoản thành công !");
        }

        [HttpPost("register")]
        public async Task<ReturnApi> RegisterUserAsync(User user)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            var validationResult = ValidateUser(user);
            if (!validationResult.success)
                return validationResult;

            if (await _userService.GetByEmailAsync(user.email!) != null)
                return new ReturnApi(false, "Email đã tồn tại !");

            if (await _userService.GetByIdentifierAsync(user.identifier!))
                return new ReturnApi(false, "CMND/CCCD đã tồn tại !");

            user.password = AuthService.HashPassword(user.password);
            var createUserResult = await _userService.CreateAsync(user);
            if (createUserResult == null)
                return new ReturnApi(false);

            createUserResult.password = null;
            return new ReturnApi(true, "Đăng ký tài khoản thành công!", createUserResult);
        }

        [HttpPost("login")]
        public async Task<ReturnApi> LoginAsync([FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (password.Length == 0)
                return new ReturnApi(false, "Mật khẩu không được bỏ trống !");

            User user = await _userService.GetByEmailAsync(email);

            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            if (AuthService.VerifyPassword(user.password, password))
            {
                user.token = Utils.GetJWT(user.id, "day", 7);
                user.password = null;
                return new ReturnApi(true, "Đăng nhập thành công !", user);
            }
            return new ReturnApi(false, "Mật khẩu không chính xác !");
        }
    }
}