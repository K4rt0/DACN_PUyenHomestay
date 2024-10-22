using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/branch")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<ReturnApi> GetBranchesAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Lấy dữ liệu thành công !", await _branchService.GetAllAsync());
        }

        [HttpGet("get-pagination")]
        public async Task<ReturnApi> GetBranchesPaginationAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "")
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if(page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            ICollection<Branch> branches = await _branchService.GetAllAsync();
            if (branches.Count == 0)
                return new ReturnApi(true, "Không có chi nhánh nào !", new
                {
                    totalItems = 0,
                    totalPages = 0,
                    currentPage = 1,
                    pageSize,
                    items = new List<Branch>()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                branches = branches.Where(branch => 
                    branch.name!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || branch.address!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || branch.province!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || branch.district!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || branch.ward!.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Cập nhật mới nhất") branches = branches.OrderByDescending(branch => branch.updated_at).ToList();
            else if (filter == "Cập nhật cũ nhất") branches = branches.OrderBy(branch => branch.updated_at).ToList();
            else if (filter == "Đang mở") branches = branches.Where(branch => !branch.is_locked).ToList();
            else if (filter == "Đã đóng") branches = branches.Where(branch => branch.is_locked).ToList();

            int totalItems = branches.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ICollection<Branch> branchesInPage = branches
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = branchesInPage
            });
        }


        [HttpGet("get-by-id")]
        public async Task<ReturnApi> GetBranchByIdAsync([FromQuery] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Branch? branch = await _branchService.GetByIdAsync(id);
            if (branch == null)
                return new ReturnApi(false, "Không tìm thấy chi nhánh này !");
            return new ReturnApi(true, "Lấy dữ liệu thành công !", branch);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPost]
        public async Task<ReturnApi> CreateBranchAsync([FromBody] Branch branch)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Branch? newBranch = await _branchService.CreateAsync(branch);
            if (newBranch == null)
                return new ReturnApi(false, "Tạo chi nhánh không thành công !");

            return new ReturnApi(true, "Tạo chi nhánh thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("{id}")]
        public async Task<ReturnApi> UpdateBranchAsync([FromRoute] int id, [FromBody] Branch branch)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (await _branchService.GetByIdAsync(id) == null)
                return new ReturnApi(false, "Không tìm thấy chi nhánh này !");

            if(await _branchService.UpdateAsync(id, branch) == null)
                return new ReturnApi(false, "Cập nhật chi nhánh không thành công !");

            return new ReturnApi(true, "Cập nhật chi nhánh thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("toggle-branch/{id}")]
        public async Task<ReturnApi> ToggleBranchAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Branch? branch = await _branchService.GetByIdAsync(id);
            if (branch == null)
                return new ReturnApi(false, "Không tìm thấy chi nhánh này !");

            Branch branchUpdated = await _branchService.ToggleBranchAsync(id);
            if(branchUpdated == null)
                return new ReturnApi(false, "Cập nhật trạng thái chi nhánh không thành công !");

            return new ReturnApi(true, "Cập nhật trạng thái chi nhánh thành công !", branchUpdated);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpDelete("{id}")]
        public async Task<ReturnApi> DeleteBranchAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Branch? branch = await _branchService.GetByIdAsync(id);
            if (branch == null)
                return new ReturnApi(false, "Không tìm thấy chi nhánh này !");

            Branch branchDeleted = await _branchService.DeleteAsync(id, branch);
            if(branchDeleted == null)
                return new ReturnApi(false, "Xóa chi nhánh không thành công !");

            return new ReturnApi(true, "Xóa chi nhánh thành công !", branchDeleted);
        }
    }
}