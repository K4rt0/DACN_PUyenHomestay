using Backend.DataAccess;
using Backend.Extensions;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class RoomImageRepository : IRoomImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ImgurService _imgurService;
        public RoomImageRepository(ApplicationDbContext context, ImgurService imgurService)
        {
            _context = context;
            _imgurService = imgurService;
        }

        public async Task<ICollection<RoomImage>> GetAllAsync()
        {
            return await _context.RoomImages!.ToListAsync();
        }

        public async Task<RoomImage> GetByIdAsync(int id)
        {
            return await _context.RoomImages!.FirstOrDefaultAsync(r => r.id == id) ?? null!;
        }

        public async Task<RoomImage> CreateAsync(RoomImage roomImage)
        {
            if (roomImage == null)
                return null!;

            roomImage.created_at = DateTime.UtcNow;
            roomImage.updated_at = DateTime.UtcNow;
            await _context.RoomImages!.AddAsync(roomImage);
            await _context.SaveChangesAsync();
            return roomImage;
        }

        public async Task<RoomImage> DeleteAsync(int id)
        {
            RoomImage? roomImage = await _context.RoomImages!.FirstOrDefaultAsync(r => r.id == id);
            if (roomImage == null)
                return null!;

            await _imgurService.DeleteImageAsync(roomImage.url!);
            _context.RoomImages!.Remove(roomImage);
            await _context.SaveChangesAsync();
            return roomImage;
        }
    }
}