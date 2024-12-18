using System.Diagnostics;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
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

            if (!string.IsNullOrEmpty(user.phone_number) && !Utils.IsValidPhoneNumber(user.phone_number.Trim()))
                return new ReturnApi(false, "Số điện thoại không hợp lệ !");

            return new ReturnApi(true);
        }
        #endregion

        [JwtAuthorize]
        [HttpPut("request-password")]
        public async Task<ReturnApi> RequestPasswordAsync(string password, string retype_password)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (password.Length == 0 || retype_password.Length == 0)
                return new ReturnApi(false, "Mật khẩu không được bỏ trống !");

            if (!password.Equals(retype_password))
                return new ReturnApi(false, "Mật khẩu không trùng khớp !");

            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false);

            user.password = AuthService.HashPassword(password);

            user = await _userService.UpdateAsync(user);
            if (user == null)
                return new ReturnApi(false, "Đã có lỗi xảy ra trong quá trình cập nhật mật khẩu !");

            return new ReturnApi(true, "Cập nhật mật khẩu thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet]
        public async Task<ReturnApi> GetAllAsync()
        {
            return new ReturnApi(true, "Lấy danh sách tài khoản thành công !", await _userService.GetAllAsync());
        }

        [JwtAuthorize]
        [HttpGet("auth")]
        public ReturnApi AuthorizeAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Xác thực thành công !");
        }

        [JwtAuthorize]
        [HttpGet("profile")]
        public async Task<ReturnApi> GetProfileByIdAsync()
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            User? existingUser = await _userService.GetProfileByIdAsync(user.id);
            if (existingUser == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");


            user.password = null;
            return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", user);

        }

        [JwtAuthorize]
        [HttpGet("get-by-token")]
        public async Task<ReturnApi> GetByTokenAsync()
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            User? existingUser = await _userService.GetByIdAsync(user.id);
            if (existingUser == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            user.password = null;
            return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", user);
        }

        [JwtAuthorize]
        [HttpGet("get-by-id")]
        public async Task<ReturnApi> GetByIdAsync(int id)
        {
            User? existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            existingUser.password = null;
            return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", existingUser);
        }

        [HttpGet("get-by-email")]
        public async Task<ReturnApi> GetByEmailAsync(string email)
        {
            User? existingUser = await _userService.GetByEmailAsync(email);
            if (existingUser == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            existingUser.password = null;
            return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", existingUser);
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
            if (!string.IsNullOrEmpty(user.email)) existingUser.email = user.email;
            if (!string.IsNullOrEmpty(user.full_name)) existingUser.full_name = user.full_name;
            if (!string.IsNullOrEmpty(user.phone_number)) existingUser.phone_number = user.phone_number;

            existingUser = await _userService.UpdateAsync(existingUser);

            if (existingUser == null)
                return new ReturnApi(false, "Đã có lỗi xảy ra trong quá trình cập nhật thông tin tài khoản !");

            return new ReturnApi(true, "Cập nhật thông tin tài khoản thành công !");
        }

        [JwtAuthorize]
        [HttpPut("change-password")]
        public async Task<ReturnApi> UpdatePasswordAsync(string old_password, string new_password, string retype_new_password)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (new_password.Length == 0)
                return new ReturnApi(false, "Mật khẩu không được bỏ trống !");

            if (!new_password.Equals(retype_new_password))
                return new ReturnApi(false, "Mật khẩu không trùng khớp !");

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
        [HttpPut("update-profile")]
        public async Task<ReturnApi> UpdateProfileAsync(string full_name, string phone_number)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (string.IsNullOrEmpty(full_name) && string.IsNullOrEmpty(phone_number))
                return new ReturnApi(false, "Không có thông tin nào được cập nhật !");

            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false);

            if (user.is_verified == false)
                return new ReturnApi(false, "Bạn cần xác thực tài khoản trước mới có thể thay đổi thông tin cá nhân !");

            if (!string.IsNullOrEmpty(full_name))
                user.full_name = full_name;
            if (!string.IsNullOrEmpty(phone_number))
                user.phone_number = phone_number;

            user = await _userService.UpdateAsync(user);
            if (user == null)
                return new ReturnApi(false, "Đã có lỗi xảy ra trong quá trình cập nhật thông tin tài khoản !");

            return new ReturnApi(true, "Cập nhật thông tin tài khoản thành công !");
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

            user.password = AuthService.HashPassword(user.password);
            var createUserResult = await _userService.CreateAsync(user);
            if (createUserResult == null)
                return new ReturnApi(false);

            createUserResult.password = "";
            return new ReturnApi(true, "Đăng ký tài khoản thành công!", createUserResult);
        }

        [HttpPost("login")]
        public async Task<ReturnApi> LoginAsync(string email, string password, bool remember = false)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (password.Length == 0)
                return new ReturnApi(false, "Mật khẩu không được bỏ trống !");

            User user = await _userService.GetByEmailAsync(email);

            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            if (AuthService.VerifyPassword(user.password, password))
                return new ReturnApi(true, "Đăng nhập thành công !", new { token = Utils.GetJWT("day", remember ? 100 : 1, new Dictionary<string, object> { { "user_id", user.id.ToString() } }) });
            return new ReturnApi(false, "Mật khẩu không chính xác !");
        }

        [JwtAuthorize]
        [HttpGet("is-change-pwd")]
        public async Task<ReturnApi> IsChangePwdAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? user = await GetCurrentUserAsync();

            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            if (AuthService.VerifyPassword(user.password, "googlelogin"))
                return new ReturnApi(true, "Đây là lần đầu đăng nhập, vui lòng đổi mật khẩu !", true);
            return new ReturnApi(false, "Đã đổi mật khẩu !");
        }

        [JwtAuthorize]
        [HttpPost("send-verification-email")]
        public async Task<ReturnApi> SendVerificationEmail()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false);

            if (user.is_verified == true)
                return new ReturnApi(false, "Tài khoản đã được xác thực !");

            var customData = new Dictionary<string, object> {
                { "email", user.email! },
                { "user_id", user.id }
            };
            var token = Utils.GetJWT("minute", 5, customData);

            var verificationLink = $"http://localhost:5173/verify-account?token={token}";

            // Lấy nội dung HTML email
            string? emailBody = SendEmail.GetVerificationEmailTemplate(user.full_name!, verificationLink);

            await _emailService.SendEmailAsync(user.email!, "Xác thực tài khoản", emailBody!);
            return new ReturnApi(true, "Vui lòng kiểm tra email để xác thực tài khoản !");
        }

        [HttpGet("verify-email")]
        public async Task<ReturnApi> VerifyEmail(string jwt_token)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            JWToken? jWToken = Utils.ParseJWT(jwt_token);

            if (jWToken == null)
                return new ReturnApi(true, "Đang xác thực tài khoản...", new
                {
                    icon = "fa-regular fa-face-thinking",
                    color = "text-warning"
                });
            if (jWToken.is_expired)
                return new ReturnApi(true, "Mã xác thực đã hết hạn !", new
                {
                    icon = "fa-solid fa-seal-exclamation",
                    color = "text-warning"
                });

            var token_data = jWToken.custom_data?.ToList();

            User user = await _userService.GetByIdAsync(int.Parse(token_data![1].Value.ToString()!));

            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            if (user.email != token_data![0].Value.ToString())
                return new ReturnApi(false, "Email không hợp lệ !");

            user.is_verified = true;
            user = await _userService.UpdateAsync(user);

            return new ReturnApi(true, "Xác thực tài khoản thành công !", new
            {
                icon = "fa-solid fa-badge-check",
                color = "text-success"
            });
        }
    }
}