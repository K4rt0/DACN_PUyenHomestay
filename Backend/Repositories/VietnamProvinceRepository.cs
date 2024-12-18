using Backend.DataAccess;
using Backend.Models.VietnamProvinces;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class VietnamProvinceRepository : IVietnamProvinceService
    {
        private readonly ApplicationDbContext _context;
        public VietnamProvinceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Province>> GetAllProvinceAsync()
        {
            return await _context.Provinces!.ToListAsync();
        }

        public async Task<ICollection<District>> GetDistrictsByProvinceIdAsync(int province_id)
        {
            return await _context.Districts!.Where(d => d.province!.id == province_id).ToListAsync();
        }

        public async Task<ICollection<Ward>> GetWardsByDistrictIdAsync(int district_id)
        {
            return await _context.Wards!.Where(w => w.district!.id == district_id).ToListAsync();
        }
    }
}