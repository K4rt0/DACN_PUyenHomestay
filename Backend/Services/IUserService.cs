using Backend.Models;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<ICollection<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User> DeleteAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<bool> GetByPhoneNumberAsync(string phone_number);
        Task<bool> GetByIdentifierAsync(string identifier);
    }
}