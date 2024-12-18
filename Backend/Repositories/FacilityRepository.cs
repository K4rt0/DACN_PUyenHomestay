using Backend.DataAccess;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class FacilityRepository : IFacilityService
    {
        private readonly ApplicationDbContext _context;
        public FacilityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Facility>> GetAllAsync()
        {
            return await _context.Facilities!.ToListAsync();
        }
        public async Task<Facility> GetByIdAsync(int id)
        {
            return await _context.Facilities!.FirstOrDefaultAsync(f => f.id == id) ?? null!;
        }

        public async Task<Facility> CreateAsync(Facility facility)
        {
            if (facility == null)
                return null!;

            facility.created_at = DateTime.UtcNow;
            facility.updated_at = DateTime.UtcNow;

            await _context.Facilities!.AddAsync(facility);
            await _context.SaveChangesAsync();
            return facility;
        }

        public async Task<Facility> UpdateAsync(int id, Facility facility)
        {
            if (facility == null)
                return null!;

            Facility? existingFacility = await _context.Facilities!.FirstOrDefaultAsync(f => f.id == id);
            if (existingFacility == null)
                return null!;

            existingFacility.icon = facility.icon;
            existingFacility.name = facility.name;
            existingFacility.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingFacility;
        }

        public async Task<Facility> DeleteAsync(int id)
        {
            Facility? existingFacility = await _context.Facilities!.FirstOrDefaultAsync(f => f.id == id);
            if (existingFacility == null)
                return null!;

            _context.Facilities!.Remove(existingFacility);
            await _context.SaveChangesAsync();
            return existingFacility;
        }
    }
}