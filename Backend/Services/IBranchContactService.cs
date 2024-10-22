using Backend.Models;

namespace Backend.Services
{
    public interface IBranchContactService
    {
        Task<ICollection<BranchContact>> GetAllAsync();
        Task<BranchContact> GetByIdAsync(int id);
        Task<BranchContact> CreateAsync(BranchContact branchContact);
        Task<BranchContact> UpdateAsync(int id, BranchContact branchContact);
        Task<BranchContact> DeleteAsync(int id, BranchContact branchContact);
    }
}