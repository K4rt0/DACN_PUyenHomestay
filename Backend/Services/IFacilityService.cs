using Backend.Models;

namespace Backend.Services
{
    public interface IFacilityService
    {
        Task<ICollection<Facility>> GetAllAsync();
        Task<Facility> GetByIdAsync(int id);
        Task<Facility> CreateAsync(Facility facility);
        Task<Facility> UpdateAsync(int id, Facility facility);
        Task<Facility> DeleteAsync(int id);
    }
}