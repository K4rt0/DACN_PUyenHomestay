using Backend.Models;

namespace Backend.Services
{
    public interface IBranchService
    {
        Task<ICollection<Branch>> GetAllAsync();
        Task<ICollection<Branch>> GetBranchForFooterAsync();
        Task<Branch> GetByIdAsync(int id);
        Task<bool> IsBranchExistAsync(string branch_name);
        Task<Branch> CreateAsync(Branch branch);
        Task<Branch> UpdateAsync(int id, Branch branch);
        Task<Branch> ToggleBranchAsync(int id);
        Task<Branch> DeleteAsync(int id, Branch branch);
    }
}