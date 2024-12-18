using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using DotEnv.Core;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;

        public AdminController(IUserService userService, IBranchService branchService)
        {
            _userService = userService;
            _branchService = branchService;
        }

        private int GetCurrentUserAsync()
        {
            if (HttpContext.Items.TryGetValue("user_id", out object? userId) &&
                int.TryParse(userId?.ToString(), out int id))
            {
                return id;
            }
            return -1;
        }
        private bool isRoot()
        {
            if (HttpContext.Items.TryGetValue("is_root", out object? isRootObj) && bool.TryParse(isRootObj?.ToString(), out bool result))
            {
                return result;
            }
            return false;
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet("auth")]
        public ReturnApi AuthorizeAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Xác thực thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet("me")]
        public async Task<ReturnApi> GetMeAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            int user_id = GetCurrentUserAsync();

            if (user_id == -1)
                return new ReturnApi(false, "Tài khoản không tồn tại !");
            else if (user_id == 0)
                return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", new
                {
                    full_name = "Root",
                    role = "Root"
                });

            User? user = await _userService.GetByIdAsync(user_id);

            if (user == null)
                return new ReturnApi(false, "Tài khoản không tồn tại !");

            return new ReturnApi(true, "Lấy thông tin tài khoản thành công !", new
            {
                full_name = user.full_name,
                role = user.role == UserRole.Staff ? "Staff" : "Manager"
            });
        }

        [JwtAuthorize(UserRole.Manager)]
        [HttpPut("update-role")]
        public async Task<ReturnApi> AddRoleAsync(int id, int role, int branch, string type, string from = "")
        {
            if (!ModelState.IsValid || (from == "root" && !isRoot()))
                return new ReturnApi(false);

            User? user = await _userService.GetByIdAsync(id);
            if (user == null)
                return new ReturnApi(false, "Không tìm thấy người dùng này !");

            if (type == "take")
            {
                user.role = UserRole.Customer;
                user.branch = null;
            }
            else if (type == "make")
            {
                Branch branch_existing = new Branch();

                if (from == "customer")
                {
                    int user_id = GetCurrentUserAsync();
                    User? user_manager = await _userService.GetByIdAsync(user_id);
                    if (user_manager == null || user_manager.branch == null || user_manager.role <= user.role)
                        return new ReturnApi(false);

                    branch_existing = await _branchService.GetByIdAsync(user_manager.branch.id);
                }
                else
                    branch_existing = await _branchService.GetByIdAsync(branch);

                user.role = (UserRole)role;
                if (branch_existing == null)
                    return new ReturnApi(false, "Không tìm thấy chi nhánh này !");
                user.branch = branch_existing;
            }

            user.updated_at = DateTime.Now;
            user = await _userService.UpdateAsync(user);
            if (user != null)
                return new ReturnApi(true, "Cập nhật vai trò thành công !");
            return new ReturnApi(false, "Cập nhật vai trò thất bại !");
        }

        [JwtAuthorize(UserRole.Manager)]
        [HttpGet("get-users-pagination/{role}")]
        public async Task<ReturnApi> GetUsersPaginationAsync(string role, [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "", [FromQuery] int filter_branch = 0)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            ICollection<User> users;
            if (role != "staffs" && role != "customer")
                return new ReturnApi(false, "Vai trò không hợp lệ !");

            if (role == "staffs")
            {
                users = await _userService.GetAllAsync();
                users = users.Where(p => p.role != UserRole.Customer).ToList();
            }
            else
                users = await _userService.GetAllByRoleAsync(UserRole.Customer);

            if (users.Count == 0)
                return new ReturnApi(true, "Không có khách hàng nào !", new
                {
                    totalItems = 0,
                    totalPages = 0,
                    currentPage = 1,
                    pageSize,
                    items = new List<User>()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(user =>
                    user.full_name!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (user.phone_number != null && user.phone_number.Contains(search, StringComparison.OrdinalIgnoreCase))
                || user.email!.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Quản lý") users = users.Where(user => user.role == UserRole.Manager).ToList();
            else if (filter == "Nhân viên") users = users.Where(user => user.role == UserRole.Staff).ToList();

            if (filter_branch != 0)
            {
                users = users.Where(user => user.branch != null && user.branch!.id == filter_branch).ToList();
            }

            int totalItems = users.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ICollection<User> usersInPage = users
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = usersInPage
            });
        }

        [JwtAuthorize(UserRole.Manager)]
        [HttpGet("get-staffs-pagination")]
        public async Task<ReturnApi> GetStaffsPaginationAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "", [FromQuery] int filter_branch = 0)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            int user_id = GetCurrentUserAsync();
            User? user = await _userService.GetByIdAsync(user_id);
            if (user == null)
                return new ReturnApi(false, "Không tìm thấy quản trị viên !");
            if (user.branch == null)
                return new ReturnApi(false);

            if (page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            ICollection<User> users = await _userService.GetAllAsync();
            users = users.Where(p => p.branch != null && p.branch.id == user.branch.id).ToList();

            if (users.Count == 0)
                return new ReturnApi(true, "Không có khách hàng nào !", new
                {
                    totalItems = 0,
                    totalPages = 0,
                    currentPage = 1,
                    pageSize,
                    items = new List<User>()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(user =>
                    user.full_name!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (user.phone_number != null && user.phone_number.Contains(search, StringComparison.OrdinalIgnoreCase))
                || user.email!.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Quản lý") users = users.Where(user => user.role == UserRole.Manager).ToList();
            else if (filter == "Nhân viên") users = users.Where(user => user.role == UserRole.Staff).ToList();

            if (filter_branch != 0)
            {
                users = users.Where(user => user.branch != null && user.branch!.id == filter_branch).ToList();
            }

            int totalItems = users.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ICollection<User> usersInPage = users
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = usersInPage
            });
        }


        [HttpGet("get-branches")]
        public async Task<ReturnApi> GetBranchesAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            ICollection<Branch> branches = await _branchService.GetAllAsync();
            return new ReturnApi(true, "Lấy dữ liệu thành công !", branches);
        }

        [JwtAuthorize(UserRole.Manager)]
        [HttpGet("get-user-by-id")]
        public async Task<ReturnApi> GetUserByIdAsync([FromQuery] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? user = await _userService.GetByIdAsync(id);

            if (user == null)
                return new ReturnApi(false, "Không tìm thấy khách hàng !");

            user.password = null;
            return new ReturnApi(true, "Lấy thông tin khách hàng thành công !", user);
        }

        [JwtAuthorize(UserRole.Manager)]
        [HttpPut("active-user")]
        public async Task<ReturnApi> ActiveUserAsync(int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? user = await _userService.GetByIdAsync(id);

            if (user == null)
                return new ReturnApi(false, "Không tìm thấy khách hàng !");

            user.is_verified = true;
            user.updated_at = DateTime.Now;

            user = await _userService.UpdateAsync(user);
            if (user != null)
                return new ReturnApi(true, "Xác thực tài khoản thành công !");

            return new ReturnApi(false, "Xác thực tài khoản thất bại !");
        }

        [JwtAuthorize(UserRole.Manager)]
        [HttpPut("lock-user")]
        public async Task<ReturnApi> LockUserAsync(int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            User? user = await _userService.GetByIdAsync(id);

            if (user == null)
                return new ReturnApi(false, "Không tìm thấy khách hàng !");

            user.is_locked = !user.is_locked;
            user.updated_at = DateTime.Now;

            user = await _userService.UpdateAsync(user);
            if (user != null)
                return new ReturnApi(true, $"{(!user.is_locked ? "Mở khoá" : "Khoá")} tài khoản thành công !");

            return new ReturnApi(false, $"{(!user!.is_locked ? "Mở khoá" : "Khoá")} tài khoản thất bại !");
        }

        #region Login
        [HttpPost("login")]
        [ServiceFilter(typeof(RateLimitFilter))]
        public async Task<ReturnApi> LoginAsync([FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            string ad_username = EnvReader.Instance["ROOT_ADMIN_USER"];
            string ad_password = EnvReader.Instance["ROOT_ADMIN_PWD"];

            if (email == ad_username && password == ad_password)
            {
                return new ReturnApi(true, "Đăng nhập thành công !", new
                {
                    token = Utils.GetJWT("year", 1,
                        new Dictionary<string, object> {
                            { "user_id", "0" },
                            { "role", "root" }
                        }),
                    role = "root"
                });
            }

            if (email.Length == 0 || password.Length == 0)
                return new ReturnApi(false, "Tài khoản hoặc mật khẩu không được bỏ trống !");

            User user = await _userService.GetByEmailAsync(email);

            if (user == null || user.role == UserRole.Customer)
                return new ReturnApi(false, "Tài khoản hoặc mật khẩu không hợp lệ !");

            if (AuthService.VerifyPassword(user.password, password))
            {
                user.password = null;
                return new ReturnApi(true, "Đăng nhập thành công !", new
                {
                    token = Utils.GetJWT("year", 1,
                    new Dictionary<string, object> {
                        { "user_id", user.id.ToString() },
                        { "role", user.id.ToString() }
                    }),
                    role = user.role == UserRole.Staff ? "Staff" : "Manager"
                });
            }
            return new ReturnApi(false, "Mật khẩu không chính xác !");
        }
        #endregion
    }
}