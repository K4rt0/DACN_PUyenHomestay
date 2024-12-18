using Backend.DataAccess;
using Backend.Extensions;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class BranchRepository : IBranchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBranchContactDetailService _branchContactDetailService;
        private readonly IBranchContactService _branchContactService;
        public BranchRepository(ApplicationDbContext context, IBranchContactService branchContactService, IBranchContactDetailService branchContactDetailService)
        {
            _context = context;
            _branchContactService = branchContactService;
            _branchContactDetailService = branchContactDetailService;
        }
        public async Task<ICollection<Branch>> GetAllAsync()
        {
            return await _context.Branches!
                    .Include(bcd => bcd.branch_contact_details!)
                    .ToListAsync();
        }
        public async Task<ICollection<Branch>> GetBranchForFooterAsync()
        {
            return await _context.Branches!
                    .Include(bcd => bcd.branch_contact_details)
                    .ThenInclude(bc => bc.branch_contact)
                    .Where(w => w.is_locked == false)
                    .ToListAsync();
        }
        public async Task<Branch> GetByIdAsync(int id)
        {
            return await _context.Branches!
                    .Include(bcd => bcd.branch_contact_details)
                    .ThenInclude(bc => bc.branch_contact)
                    .FirstOrDefaultAsync(f => f.id == id) ?? null!;
        }
        public async Task<bool> IsBranchExistAsync(string branch_name)
        {
            return await _context.Branches!.AnyAsync(b => b.name == branch_name);
        }
        public async Task<Branch> CreateAsync(Branch branch)
        {
            if (branch == null)
                return null!;

            branch.created_at = DateTime.UtcNow;
            branch.updated_at = DateTime.UtcNow;
            ICollection<BranchContactDetail> branch_contact_detail = branch.branch_contact_details!;
            branch.branch_contact_details = new HashSet<BranchContactDetail>();

            await _context.Branches!.AddAsync(branch);
            foreach (var f in branch_contact_detail)
            {
                f.branch = branch;
                await _branchContactDetailService.CreateAsync(f);
            }

            await _context.SaveChangesAsync();
            return branch;
        }
        public async Task<Branch> DeleteAsync(int id, Branch branch)
        {
            if (branch == null)
                return null!;

            if (await _context.Branches!.FirstOrDefaultAsync(f => f.id == branch.id) == null)
                return null!;

            foreach (var contactDetail in branch.branch_contact_details.ToList())
            {
                await _branchContactDetailService.DeleteAsync(contactDetail);
            }

            _context.Branches!.Remove(branch);
            await _context.SaveChangesAsync();

            return branch;
        }
        public async Task<Branch> UpdateAsync(int id, Branch branch)
        {
            if (branch == null)
                return null!;

            Branch? existingBranch = await _context.Branches!.FirstOrDefaultAsync(f => f.id == id);
            if (existingBranch == null)
                return null!;

            existingBranch.name = branch.name;
            existingBranch.address = branch.address;
            existingBranch.province = branch.province;
            existingBranch.district = branch.district;
            existingBranch.ward = branch.ward;
            existingBranch.description = branch.description;
            existingBranch.updated_at = DateTime.UtcNow;

            if (branch.branch_contact_details.Count() == 0)
                return null!;

            foreach (var bcd in existingBranch.branch_contact_details.ToList())
            {
                var item = branch.branch_contact_details.FirstOrDefault(f => f.id == bcd.id);
                if (item == null)
                {
                    await _branchContactDetailService.DeleteAsync(bcd);
                }
                else
                {
                    bcd.value = item.value;
                    bcd.branch_contact = await _branchContactService.GetByIdAsync(item.branch_contact!.id);
                    bcd.updated_at = DateTime.UtcNow;
                }
            }

            foreach (var bcd in branch.branch_contact_details.ToList())
            {
                if (bcd.id == 0)
                {
                    bcd.branch = existingBranch;
                    bcd.branch_contact = await _branchContactService.GetByIdAsync(bcd.branch_contact!.id);
                    await _branchContactDetailService.CreateAsync(bcd);
                }
            }

            _context.Branches!.Update(existingBranch);
            await _context.SaveChangesAsync();
            return existingBranch;
        }
        public async Task<Branch> ToggleBranchAsync(int id)
        {
            Branch? branch = await _context.Branches!.FirstOrDefaultAsync(f => f.id == id);
            if (branch == null)
                return null!;

            branch.is_locked = !branch.is_locked;
            await _context.SaveChangesAsync();
            return branch;
        }
    }
}