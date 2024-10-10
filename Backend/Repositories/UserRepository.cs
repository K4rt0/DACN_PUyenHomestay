using Backend.DataAccess;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class UserRepository : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(User user)
        {
            user.created_at = DateTime.UtcNow;
            user.updated_at = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id) => await _context.Users.FirstOrDefaultAsync(f => f.id == id) ?? null!;

        public async Task<User> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(f => f.email == email) ?? null!;

        public async Task<bool> GetByPhoneNumberAsync(string phone_number) => await _context.Users.FirstOrDefaultAsync(f => f.phone_number == phone_number) != null;

        public async Task<bool> GetByIdentifierAsync(string identifier) => await _context.Users.FirstOrDefaultAsync(f => f.identifier == identifier) != null;
    }
}