using Backend.Models;

namespace Backend.Services
{
    public interface IBranchContactDetailService
    {
        Task<ICollection<BranchContactDetail>> GetAllAsync();
        Task<BranchContactDetail> GetByIdAsync(int id);
        Task<BranchContactDetail> CreateAsync(BranchContactDetail branchContactDetail);
        Task<BranchContactDetail> DeleteAsync(BranchContactDetail branchContactDetail);
        Task<BranchContactDetail> UpdateAsync(BranchContactDetail branchContactDetail);
    }
}