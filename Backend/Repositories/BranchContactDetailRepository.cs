using Backend.DataAccess;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class BranchContactDetailRepository : IBranchContactDetailService
    {
        private readonly ApplicationDbContext _context;
        public BranchContactDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<BranchContactDetail>> GetAllAsync()
        {
            return await _context.BranchContactDetails.Include(p => p.branch).ToListAsync();
        }
        public async Task<BranchContactDetail> GetByIdAsync(int id)
        {
            return await _context.BranchContactDetails.FirstOrDefaultAsync(f => f.id == id) ?? null!;
        }
        public async Task<BranchContactDetail> CreateAsync(BranchContactDetail branchContactDetail)
        {
            if (branchContactDetail == null)
                throw new ArgumentNullException(nameof(branchContactDetail));

            if (branchContactDetail.branch_contact == null || branchContactDetail.branch_contact.id == 0)
                throw new ArgumentException("Branch contact is null or has an invalid id.");

            BranchContact? branchContact = await _context.BranchContacts.FirstOrDefaultAsync(f => f.id == branchContactDetail.branch_contact.id);
            if (branchContact == null)
                throw new ArgumentException("Branch contact not found.");
                
            branchContactDetail.branch_contact = branchContact;

            await _context.BranchContactDetails.AddAsync(branchContactDetail);
            await _context.SaveChangesAsync();
            return branchContactDetail;
        }
        public async Task<BranchContactDetail> DeleteAsync(BranchContactDetail branchContactDetail)
        {
            if (branchContactDetail == null)
                return null!;

            BranchContactDetail? existingBranchContactDetail = await _context.BranchContactDetails
                .FirstOrDefaultAsync(f => f.id == branchContactDetail.id);

            if (existingBranchContactDetail != null)
            {
                _context.BranchContactDetails.Remove(existingBranchContactDetail);
                await _context.SaveChangesAsync(); // Save changes after removal
            }

            return existingBranchContactDetail ?? null!;
        }

        public async Task<BranchContactDetail> UpdateAsync(BranchContactDetail branchContactDetail)
        {
            if (branchContactDetail == null)
                return null!;

            BranchContactDetail? existingBranchContactDetail = await _context.BranchContactDetails.FirstOrDefaultAsync(f => f.id == branchContactDetail.id);
            if (existingBranchContactDetail == null)
                return null!;

            existingBranchContactDetail.value = branchContactDetail.value;
            existingBranchContactDetail.is_locked = branchContactDetail.is_locked;
            existingBranchContactDetail.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingBranchContactDetail;
        }
    }
}