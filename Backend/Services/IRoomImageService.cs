using Backend.Models;

namespace Backend.Services
{
    public interface IRoomImageService
    {
        Task<ICollection<RoomImage>> GetAllAsync();
        Task<RoomImage> GetByIdAsync(int id);
        Task<RoomImage> CreateAsync(RoomImage roomImage);
        Task<RoomImage> DeleteAsync(int id);
    }
}