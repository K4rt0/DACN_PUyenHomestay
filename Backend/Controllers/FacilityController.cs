using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/facilities")]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _facilityService;
        public FacilityController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [HttpGet]
        public async Task<ReturnApi> GetFacilities()
        {
            if(!ModelState.IsValid)
                return new ReturnApi(false);

            IEnumerable<Facility> facilities = await _facilityService.GetAllAsync();
            if(facilities.Count() == 0 || facilities == null)
                return new ReturnApi(false);
            
            return new ReturnApi(true, "Lấy dữ liệu thành công !", facilities);
        }

        [HttpGet("{id}")]
        public async Task<ReturnApi> GetFacility(int id)
        {
            if(!ModelState.IsValid)
                return new ReturnApi(false);

            Facility facility = await _facilityService.GetByIdAsync(id);
            if(facility == null)
                return new ReturnApi(false, "Không tìm thấy thông tin cần tìm !");
            
            return new ReturnApi(true, "Lấy dữ liệu thành công !", facility);
        }

        [HttpGet("get-pagination")]
        public async Task<ReturnApi> GetFacilitiesPaginationAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "")
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if(page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            ICollection<Facility> facilities = await _facilityService.GetAllAsync();
            if (facilities.Count == 0)
                return new ReturnApi(true, "Không có tiện ích nào !", new
                {
                    totalItems = 0,
                    totalPages = 0,
                    currentPage = 1,
                    pageSize,
                    items = new List<Facility>()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                facilities = facilities.Where(facility => 
                    facility.name!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    facility.icon!.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Cập nhật mới nhất") facilities = facilities.OrderByDescending(facility => facility.updated_at).ToList();
            else if (filter == "Cập nhật cũ nhất") facilities = facilities.OrderBy(facility => facility.updated_at).ToList();

            int totalItems = facilities.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ICollection<Facility> facilitiesInPage = facilities
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = facilitiesInPage
            });
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPost]
        public async Task<ReturnApi> CreateFacility([FromBody] Facility facility)
        {
            if(!ModelState.IsValid)
                return new ReturnApi(false);

            Facility newFacility = await _facilityService.CreateAsync(facility);
            if(newFacility == null)
                return new ReturnApi(false, "Tạo thông tin không thành công !");

            return new ReturnApi(true, "Tạo thông tin thành công !", newFacility);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("{id}")]
        public async Task<ReturnApi> UpdateFacility(int id, [FromBody] Facility facility)
        {
            if(!ModelState.IsValid)
                return new ReturnApi(false);

            Facility updatedFacility = await _facilityService.UpdateAsync(id, facility);
            if(updatedFacility == null)
                return new ReturnApi(false, "Cập nhật thông tin không thành công !");

            return new ReturnApi(true, "Cập nhật thông tin thành công !", updatedFacility);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpDelete("{id}")]
        public async Task<ReturnApi> DeleteFacility(int id)
        {
            if(!ModelState.IsValid)
                return new ReturnApi(false);

            Facility deletedFacility = await _facilityService.DeleteAsync(id);
            if(deletedFacility == null)
                return new ReturnApi(false, "Xóa thông tin không thành công !");

            return new ReturnApi(true, "Xóa thông tin thành công !", deletedFacility);
        }
    }
}