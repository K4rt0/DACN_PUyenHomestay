using Backend.DataAccess;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class BranchContactRepository : IBranchContactService
    {
        private readonly ApplicationDbContext _context;

        public BranchContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<BranchContact>> GetAllAsync()
        {
            return await _context.BranchContacts.ToListAsync();
        }

        public async Task<BranchContact> GetByIdAsync(int id)
        {
            return await _context.BranchContacts.FirstOrDefaultAsync(f => f.id == id) ?? null!;
        }

        public async Task<BranchContact> CreateAsync(BranchContact branchContact)
        {
            if (branchContact == null)
                return null!;

            branchContact.created_at = DateTime.UtcNow;
            branchContact.updated_at = DateTime.UtcNow;

            await _context.BranchContacts.AddAsync(branchContact);
            await _context.SaveChangesAsync();
            return branchContact;
        }

        public async Task<BranchContact> DeleteAsync(int id, BranchContact branchContact)
        {
            if (branchContact == null)
                return null!;

            BranchContact? existingBranchContact = await _context.BranchContacts.FirstOrDefaultAsync(f => f.id == id);
            if(existingBranchContact == null)
                return null!;

            _context.BranchContacts.Remove(branchContact);
            await _context.SaveChangesAsync();
            return existingBranchContact ?? null!;
        }

        public async Task<BranchContact> UpdateAsync(int id, BranchContact branchContact)
        {
            if (branchContact == null)
                return null!;

            BranchContact? existingBranchContact = await _context.BranchContacts.FirstOrDefaultAsync(f => f.id == id);
            if (existingBranchContact == null)
                return null!;

            existingBranchContact.name = branchContact.name;
            existingBranchContact.contact_icon = branchContact.contact_icon;
            existingBranchContact.updated_at = DateTime.UtcNow;

            _context.BranchContacts.Update(existingBranchContact);
            await _context.SaveChangesAsync();
            return existingBranchContact;
        }
    }
}