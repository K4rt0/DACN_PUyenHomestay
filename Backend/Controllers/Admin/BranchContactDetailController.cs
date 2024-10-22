using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Admin
{
    [ApiController]
    [Route("api/branch-contact-detail")]
    public class BranchContactDetailController : ControllerBase
    {
        private readonly IBranchContactDetailService _branchContactDetailService;
        public BranchContactDetailController(IBranchContactDetailService branchContactDetailService)
        {
            _branchContactDetailService = branchContactDetailService;
        }

        [HttpGet]
        public async Task<ReturnApi> GetBranchContactDetailsAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Lấy dữ liệu thành công !", await _branchContactDetailService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ReturnApi> GetBranchContactDetailByIdAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            BranchContactDetail? branchContactDetail = await _branchContactDetailService.GetByIdAsync(id);
            if (branchContactDetail == null)
                return new ReturnApi(false, "Không tìm thấy chi tiết liên hệ này !");
            return new ReturnApi(true, "Lấy dữ liệu thành công !", branchContactDetail);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPost]
        public async Task<ReturnApi> CreateBranchContactDetailAsync([FromBody] BranchContactDetail branchContactDetail)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            BranchContactDetail? newBranchContactDetail = await _branchContactDetailService.CreateAsync(branchContactDetail);
            if (newBranchContactDetail == null)
                return new ReturnApi(false, "Tạo chi tiết liên hệ không thành công !");

            return new ReturnApi(true, "Tạo chi tiết liên hệ thành công !", newBranchContactDetail);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("{id}")]
        public async Task<ReturnApi> UpdateBranchContactDetailAsync([FromRoute] int id, [FromBody] BranchContactDetail branchContactDetail)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (id != branchContactDetail.id || await _branchContactDetailService.GetByIdAsync(id) == null)
                return new ReturnApi(false, "Không tìm thấy chi tiết liên hệ này !");

            BranchContactDetail? updatedBranchContactDetail = await _branchContactDetailService.UpdateAsync(branchContactDetail);
            if (updatedBranchContactDetail == null)
                return new ReturnApi(false, "Cập nhật chi tiết liên hệ không thành công !");
            return new ReturnApi(true, "Cập nhật chi tiết liên hệ thành công !", updatedBranchContactDetail);
        }
    }
}