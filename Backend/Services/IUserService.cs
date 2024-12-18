using Backend.Models;
using Backend.Models.Enums;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<ICollection<User>> GetAllAsync();
        Task<ICollection<User>> GetAllByRoleAsync(UserRole role);
        Task<User> GetByIdAsync(int id);
        Task<User> GetProfileByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User> DeleteAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<bool> GetByPhoneNumberAsync(string phone_number);
    }
}