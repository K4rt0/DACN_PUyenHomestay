using Backend.DataAccess;
using Backend.Models;
using Backend.Models.Enums;
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

            await _context.Users!.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users!.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> DeleteAsync(User user)
        {
            _context.Users!.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            ICollection<User> users = await _context.Users!
                .Include(p => p.branch)
                .ToListAsync();
            users.ToList().ForEach(u => u.password = null);
            return users;
        }

        public async Task<User> GetByIdAsync(int id) => await _context.Users!.Include(b => b.branch).FirstOrDefaultAsync(f => f.id == id) ?? null!;
        public async Task<User> GetProfileByIdAsync(int id)
        {
            var user = await _context.Users!
                .Include(p => p.reservations)
                .FirstOrDefaultAsync(f => f.id == id);

            if (user != null && user.reservations.Any())
            {
                await _context.Entry(user)
                    .Collection(u => u.reservations)
                    .Query()
                    .Include(r => r.room)
                    .ThenInclude(r => r!.room_images)
                    .Include(r => r.room)
                    .ThenInclude(r => r!.branch)
                    .LoadAsync();
            }

            return user ?? null!;
        }

        public async Task<ICollection<User>> GetAllByRoleAsync(UserRole role)
        {
            ICollection<User> users = await _context.Users!.
            Include(p => p.branch).Where(w => w.role == role).ToListAsync();
            users.ToList().ForEach(u => u.password = null);
            return users;
        }

        public async Task<User> GetByEmailAsync(string email) => await _context.Users!.FirstOrDefaultAsync(f => f.email == email) ?? null!;

        public async Task<bool> GetByPhoneNumberAsync(string phone_number) => await _context.Users!.FirstOrDefaultAsync(f => f.phone_number == phone_number) != null;

    }
}