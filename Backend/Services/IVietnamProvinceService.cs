using Backend.Models.VietnamProvinces;

namespace Backend.Services
{
    public interface IVietnamProvinceService
    {
        Task<ICollection<Province>> GetAllProvinceAsync();
        Task<ICollection<District>> GetDistrictsByProvinceIdAsync(int province_id);
        Task<ICollection<Ward>> GetWardsByDistrictIdAsync(int district_id);
    }
}