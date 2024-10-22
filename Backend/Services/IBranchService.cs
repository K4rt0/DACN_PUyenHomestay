using Backend.Models;

namespace Backend.Services
{
    public interface IBranchService
    {
        Task<ICollection<Branch>> GetAllAsync();
        Task<Branch> GetByIdAsync(int id);
        Task<Branch> CreateAsync(Branch branch);
        Task<Branch> UpdateAsync(int id, Branch branch);
        Task<Branch> ToggleBranchAsync(int id);
        Task<Branch> DeleteAsync(int id, Branch branch);
    }
}