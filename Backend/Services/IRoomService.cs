using Backend.Models;

namespace Backend.Services
{
    public interface IRoomService
    {
        Task<ICollection<Room>> GetAllAsync();
        Task<Room> GetByIdAsync(int id);
        Task<Room> GetRoomDetail(int room_id, int branch_id, DateTime start_date, DateTime end_date);
        Task<bool> IsRoomExistAsync(string room_number, int branch_id);
        Task<Room> CreateAsync(Room room, List<IFormFile> images);
        Task<Room> UpdateAsync(int id, Room room, List<IFormFile>? images);
        Task<Room> ToggleRoomAsync(int id);
        Task<Room> DeleteAsync(int id);
        Task<List<Room>> FindRooms(DateTime start_date, DateTime end_date, int branch_id, int room_adults);

    }
}