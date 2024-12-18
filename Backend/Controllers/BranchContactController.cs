using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/branch-contact")]
    public class BranchContactController : ControllerBase
    {
        private readonly IBranchContactService _branchContactService;
        public BranchContactController(IBranchContactService branchContactService)
        {
            _branchContactService = branchContactService;
        }

        [HttpGet]
        public async Task<ReturnApi> GetBranchContactsAsync()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Lấy dữ liệu thành công !", await _branchContactService.GetAllAsync());
        }

        [HttpGet("get-pagination")]
        public async Task<ReturnApi> GetBranchContactsAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "")
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if(page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            ICollection<BranchContact> branchContacts = await _branchContactService.GetAllAsync();
            if (branchContacts.Count == 0)
                return new ReturnApi(false, "Không có thông tin liên hệ nào !");

            if (!string.IsNullOrWhiteSpace(search))
            {
                branchContacts = branchContacts.Where(contact => contact.name!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || contact.contact_icon!.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Latest Updated")
                branchContacts = branchContacts.OrderByDescending(contact => contact.updated_at).ToList();
            else if (filter == "Oldest Updated")
                branchContacts = branchContacts.OrderBy(contact => contact.updated_at).ToList();
            int totalItems = branchContacts.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ICollection<BranchContact> branchContactsInPage = branchContacts
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = branchContactsInPage
            });
        }

        [HttpGet("{id}")]
        public async Task<ReturnApi> GetBranchContactByIdAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            BranchContact? branchContact = await _branchContactService.GetByIdAsync(id);
            if (branchContact == null)
                return new ReturnApi(false, "Không tìm thấy thông tin liên hệ này !");

            return new ReturnApi(true, "Lấy dữ liệu thành công !", branchContact);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPost]
        public async Task<ReturnApi> CreateBranchContactAsync([FromBody] BranchContact branchContact)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            BranchContact? newBranchContact = await _branchContactService.CreateAsync(branchContact);
            if (newBranchContact == null)
                return new ReturnApi(false, "Tạo thông tin liên hệ không thành công !");

            return new ReturnApi(true, "Tạo thông tin liên hệ thành công !", newBranchContact);
        }
        
        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("{id}")]
        public async Task<ReturnApi> UpdateBranchContactAsync([FromRoute] int id, [FromBody] BranchContact branchContact)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            BranchContact? branch_contact_existing = await _branchContactService.GetByIdAsync(id);
            if (branch_contact_existing == null)
                return new ReturnApi(false, "Không tìm thấy thông tin liên hệ này !");

            BranchContact branchContactUpdated = await _branchContactService.UpdateAsync(id, branchContact);
            if (branchContactUpdated == null)
                return new ReturnApi(false, "Cập nhật thông tin liên hệ không thành công !");

            return new ReturnApi(true, "Cập nhật thông tin liên hệ thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpDelete("{id}")]
        public async Task<ReturnApi> DeleteBranchContactAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            BranchContact? branchContact = await _branchContactService.GetByIdAsync(id);
            if (branchContact == null)
                return new ReturnApi(false, "Không tìm thấy thông tin liên hệ này !");

            BranchContact branchContactDeleted = await _branchContactService.DeleteAsync(id, branchContact);
            if (branchContactDeleted == null)
                return new ReturnApi(false, "Xóa thông tin liên hệ không thành công !");

            return new ReturnApi(true, "Xóa thông tin liên hệ thành công !");
        }
    }
}