using System.Text.Json;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/room")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<ReturnApi> GetRooms()
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            IEnumerable<Room> rooms = await _roomService.GetAllAsync();
            if (rooms.Count() == 0 || rooms == null)
                return new ReturnApi(false);

            return new ReturnApi(true, "Lấy dữ liệu thành công !", rooms);
        }

        [HttpGet("{id}")]
        public async Task<ReturnApi> GetRooms([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Room? room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return new ReturnApi(false);

            return new ReturnApi(true, "Lấy dữ liệu thành công !", room);
        }

        [HttpGet("search")]
        public async Task<ReturnApi> FindRooms(DateTime start_date, DateTime end_date, int branch_id, int room_adults)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            List<Room>? room = await _roomService.FindRooms(start_date, end_date, branch_id, room_adults);
            if (room == null)
                return new ReturnApi(false, "Ngày mà bạn chọn đã hết phòng trống, vui lòng chọn ngày khác hoặc xem thêm chi nhánh khác !");

            return new ReturnApi(true, "Tìm phòng thành công !", room);
        }

        [HttpGet("get-room")]
        public async Task<ReturnApi> GetRoom([FromQuery] int room_id, [FromQuery] int branch_id, [FromQuery] DateTime start_date, [FromQuery] DateTime end_date)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Room room = await _roomService.GetRoomDetail(room_id, branch_id, start_date, end_date);
            if (room == null)
                return new ReturnApi(false, "Không tìm thấy thông tin cần tìm !");

            return new ReturnApi(true, "Lấy dữ liệu thành công !", room);
        }

        [HttpGet("get-pagination")]
        public async Task<ReturnApi> GetRoomsPaginationAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "")
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            ICollection<Room> rooms = await _roomService.GetAllAsync();
            if (rooms.Count == 0)
                return new ReturnApi(true, "Không có phòng nào !", new
                {
                    totalItems = 0,
                    totalPages = 0,
                    currentPage = 1,
                    pageSize,
                    items = new List<Room>()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                rooms = rooms.Where(room =>
                    room.room_number!.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Cập nhật mới nhất") rooms = rooms.OrderByDescending(room => room.updated_at).ToList();
            else if (filter == "Cập nhật cũ nhất") rooms = rooms.OrderBy(room => room.updated_at).ToList();
            else if (filter == "Giá tăng dần") rooms = rooms.OrderBy(room => room.cost).ToList();
            else if (filter == "Giá giảm dần") rooms = rooms.OrderByDescending(room => room.cost).ToList();
            else if (filter == "Diện tích tăng dần") rooms = rooms.OrderBy(room => room.room_width).ToList();
            else if (filter == "Diện tích giảm dần") rooms = rooms.OrderByDescending(room => room.room_width).ToList();

            int totalItems = rooms.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ICollection<Room> roomsInPage = rooms
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = roomsInPage
            });
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPost]
        public async Task<ReturnApi> CreateRoom([FromForm] string roomJson, List<IFormFile> images)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Room? room = JsonSerializer.Deserialize<Room>(roomJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (room == null || room.cost == 0 || room.branch?.id == 0 || string.IsNullOrEmpty(room.room_number) || !Utils.IsValidRoomNumber(room.room_number) || room.room_width == 0)
                return new ReturnApi(false, "Dữ liệu không hợp lệ !");

            if (await _roomService.IsRoomExistAsync(room.room_number, room.branch!.id))
                return new ReturnApi(false, "Phòng này đã tồn tại !");

            await _roomService.CreateAsync(room, images);
            return new ReturnApi(true, "Thêm phòng thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("{id}")]
        public async Task<ReturnApi> UpdateRoom(int id, [FromForm] string roomJson, List<IFormFile>? images)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Room? room = JsonSerializer.Deserialize<Room>(roomJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (room == null || room.cost == 0 || string.IsNullOrEmpty(room.room_number) || !Utils.IsValidRoomNumber(room.room_number) || room.room_width == 0)
                return new ReturnApi(false, "Dữ liệu không hợp lệ !");

            if (await _roomService.GetByIdAsync(id) == null)
                return new ReturnApi(false, "Không tìm thấy phòng cần cập nhật !");

            if (await _roomService.UpdateAsync(id, room, images) == null)
                return new ReturnApi(false, "Cập nhật phòng thất bại !");

            return new ReturnApi(true, "Cập nhật phòng thành công !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut("toggle-room/{id}")]
        public async Task<ReturnApi> ToggleBranchAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Room? room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return new ReturnApi(false, "Không tìm thấy chi nhánh này !");

            Room roomUpdated = await _roomService.ToggleRoomAsync(id);
            if (roomUpdated == null)
                return new ReturnApi(false, "Cập nhật trạng thái chi nhánh không thành công !");

            return new ReturnApi(true, "Cập nhật trạng thái chi nhánh thành công !", roomUpdated);
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpDelete("{id}")]
        public async Task<ReturnApi> DeleteRoom(int id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Room room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return new ReturnApi(false, "Không tìm thấy phòng cần xóa !");

            await _roomService.DeleteAsync(id);
            return new ReturnApi(true, "Xóa phòng thành công !");
        }
    }
}