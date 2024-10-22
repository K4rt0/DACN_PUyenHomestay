using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.VietnamProvince
{
    [ApiController]
    [Route("api/vietnam")]
    public class VietnamProvinceController : ControllerBase
    {
        private readonly IVietnamProvinceService _vietnamProvinceService;
        public VietnamProvinceController(IVietnamProvinceService vietnamProvinceService)
        {
            _vietnamProvinceService = vietnamProvinceService;
        }

        [HttpGet("provinces")]
        public async Task<ReturnApi> GetAllProvinceAsync()
        {
            var provinces = await _vietnamProvinceService.GetAllProvinceAsync();
            return new ReturnApi(true, "Lấy dữ liệu thành công !", provinces);
        }

        [HttpGet("districts/{province_id}")]
        public async Task<ReturnApi> GetDistrictsByProvinceIdAsync(int province_id)
        {
            var districts = await _vietnamProvinceService.GetDistrictsByProvinceIdAsync(province_id);
            return new ReturnApi(true, "Lấy dữ liệu thành công !", districts);
        }

        [HttpGet("wards/{district_id}")]
        public async Task<ReturnApi> GetWardsByDistrictIdAsync(int district_id)
        {
            var wards = await _vietnamProvinceService.GetWardsByDistrictIdAsync(district_id);
            return new ReturnApi(true, "Lấy dữ liệu thành công !", wards);
        }
    }
}