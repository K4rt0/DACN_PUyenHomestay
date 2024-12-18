using Backend.DataAccess;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class RoomRepository : IRoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly ImgurService _imgurService;
        private readonly IRoomImageService _roomImageService;
        public RoomRepository(ApplicationDbContext context, ImgurService imgurService, IRoomImageService roomImageService)
        {
            _roomImageService = roomImageService;
            _context = context;
            _imgurService = imgurService;
        }
        public async Task<ICollection<Room>> GetAllAsync()
        {
            return await _context.Rooms!
                        .Include(b => b.branch)
                        .ToListAsync();
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms!
                        .Include(b => b.branch)
                        .Include(ri => ri.room_images)
                        .Include(f => f.facilities_rooms)
                        .ThenInclude(f => f.facility)
                        .FirstOrDefaultAsync(r => r.id == id) ?? null!;
        }

        public async Task<bool> IsRoomExistAsync(string room_number, int branch_id)
        {
            return await _context.Rooms!.AnyAsync(r => r.room_number == room_number && r.branch!.id == branch_id);
        }

        public async Task<Room> CreateAsync(Room room, List<IFormFile> images)
        {
            if (room == null)
                return null!;

            room.branch = await _context.Branches!.FirstOrDefaultAsync(b => b.id == room.branch!.id);
            if (room.branch == null)
                return null!;

            room.created_at = DateTime.UtcNow;
            room.updated_at = DateTime.UtcNow;

            room.facilities_rooms.ToList().ForEach(item =>
            {
                item.room = room;
                item.facility = _context.Facilities!.FirstOrDefault(f => f.id == item.facility!.id);
            });

            List<string> results = new List<string>();
            foreach (var image in images)
            {
                var result = await _imgurService.UploadImageAsync(image);
                if (result == null || string.IsNullOrEmpty(result.Value.imgurLink))
                    return null!;
                room.room_images.Add(new RoomImage
                {
                    url = result.Value.imgurLink,
                    type = "." + result.Value.type.Split("/")[1],
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                });
            }

            await _context.Rooms!.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> DeleteAsync(int id)
        {
            Room? room = await _context.Rooms!
                            .Include(ri => ri.room_images)
                            .Include(f => f.facilities_rooms)
                            .FirstOrDefaultAsync(r => r.id == id);
            if (room == null)
                return null!;

            foreach (var image in room.room_images)
            {
                await _imgurService.DeleteImageAsync(image.url!);
            }

            _context.RoomImages!.RemoveRange(room.room_images);
            _context.FacilitiesRooms!.RemoveRange(room.facilities_rooms);
            _context.Rooms!.Remove(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateAsync(int id, Room room, List<IFormFile>? images)
        {
            if (room == null)
                return null!;

            Room? existingRoom = await _context.Rooms!.FirstOrDefaultAsync(r => r.id == id);
            if (existingRoom == null)
                return null!;

            existingRoom.room_number = room.room_number;
            existingRoom.cost = room.cost;
            existingRoom.room_width = room.room_width;
            existingRoom.room_max_adults = room.room_max_adults;
            existingRoom.room_name = room.room_name;
            existingRoom.updated_at = DateTime.UtcNow;

            foreach (var ri in existingRoom.room_images.ToList())
            {
                var item = room.room_images.FirstOrDefault(f => f.id == ri.id);
                if (item == null)
                    await _roomImageService.DeleteAsync(ri.id);
            }
            if (images != null)
            {
                foreach (var image in images)
                {
                    var result = await _imgurService.UploadImageAsync(image);
                    if (result == null || string.IsNullOrEmpty(result.Value.imgurLink))
                        return null!;
                    existingRoom.room_images.Add(new RoomImage
                    {
                        url = result.Value.imgurLink,
                        type = result.Value.type.Replace("images/", "."),
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    });
                }
            }
            if (existingRoom.facilities_rooms != null)
                _context.FacilitiesRooms!.RemoveRange(existingRoom.facilities_rooms);
            foreach (var fr in room.facilities_rooms.ToList())
            {
                existingRoom.facilities_rooms!.Add(new FacilitiesRoom
                {
                    room = existingRoom,
                    facility = _context.Facilities!.FirstOrDefault(f => f.id == fr.facility!.id)
                });
            }
            _context.Rooms!.Update(existingRoom);
            await _context.SaveChangesAsync();
            return existingRoom;
        }

        public async Task<List<Room>> FindRooms(DateTime start_date, DateTime end_date, int branch_id, int room_adults)
        {
            var availableRooms = await _context.Rooms!
            .Where(r => r.branch!.id == branch_id && r.room_max_adults >= room_adults && !r.is_locked)
            .Where(r => !r.reservations.Any(rd =>
                rd.check_in < DateOnly.FromDateTime(end_date) && rd.check_out > DateOnly.FromDateTime(start_date) &&
                (rd.status != ReservationStatus.Failed && rd.status != ReservationStatus.Cancelled && rd.status != ReservationStatus.Pending)))
            .OrderBy(r => r.room_max_adults)
            .Include(b => b.room_images)
            .Include(b => b.facilities_rooms)
            .ThenInclude(f => f.facility)
            .Include(b => b.branch)
            .ToListAsync();
            return availableRooms;
        }

        public async Task<Room> GetRoomDetail(int room_id, int branch_id, DateTime start_date, DateTime end_date)
        {
            return await _context.Rooms!
            .Include(r => r.room_images)
            .Include(r => r.facilities_rooms)
            .ThenInclude(f => f.facility)
            .Include(r => r.room_images)
            .Include(r => r.branch)
            .FirstOrDefaultAsync(r => r.id == room_id && r.branch!.id == branch_id && !r.is_locked && !r.reservations.Any(rd =>
                rd.check_in < DateOnly.FromDateTime(end_date) && rd.check_out > DateOnly.FromDateTime(start_date) &&
                (rd.status != ReservationStatus.Failed && rd.status != ReservationStatus.Cancelled && rd.status != ReservationStatus.Pending))) ?? null!;
        }

        public async Task<Room> ToggleRoomAsync(int id)
        {
            Room? room = await _context.Rooms!.FirstOrDefaultAsync(r => r.id == id);
            if (room == null)
                return null!;

            room.is_locked = !room.is_locked;
            await _context.SaveChangesAsync();
            return room;
        }
    }
}