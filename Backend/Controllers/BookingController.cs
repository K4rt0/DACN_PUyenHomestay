using Backend.DataAccess;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Payment;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/booking")]
    public class BookingController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IMomoService _momoService;
        private readonly IUserService _userService;
        private readonly IReservationService _reservationService;
        private readonly ApplicationDbContext _context;
        public BookingController(IVnPayService vnPayService, IReservationService reservationService, IMomoService momoService, IUserService userService, ApplicationDbContext context)
        {
            _context = context;
            _reservationService = reservationService;
            _userService = userService;
            _vnPayService = vnPayService;
            _momoService = momoService;
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            if (HttpContext.Items.TryGetValue("user_id", out object? userId) &&
                int.TryParse(userId?.ToString(), out int id))
            {
                return await _userService.GetByIdAsync(id);
            }
            return null;
        }

        [JwtAuthorize]
        [HttpPost("book-room")]
        public async Task<ReturnApi> BookRoom([FromBody] Reservation reservation)
        {
            User? user = await GetCurrentUserAsync();
            if (user == null)
                return new ReturnApi(false, "Người dùng không xác định !");
            if (user.reward_points < reservation.reward_points)
                return new ReturnApi(false, "Bạn không có đủ điểm để đổi !");
            Room? room = await _context.Rooms!.FirstOrDefaultAsync(r => r.id == reservation.room!.id);
            if (!ModelState.IsValid || user == null || room == null)
                return new ReturnApi(false);

            bool isRoomBooked = await _context.Reservations!
                .AnyAsync(rd =>
                rd.check_in < (DateOnly)reservation.check_out! && rd.check_out > (DateOnly)reservation.check_in! && rd.status != ReservationStatus.Pending && rd.status != ReservationStatus.Failed && rd.status != ReservationStatus.Cancelled);

            if (isRoomBooked)
                return new ReturnApi(false, "Phòng này đã được đặt !");

            user.reward_points -= reservation.reward_points;
            reservation.booking_id = Guid.NewGuid();
            reservation.user = user;
            reservation.payment_status = reservation.payment_method == PaymentMethod.PayInHotel ? true : false;
            reservation.room = room;

            reservation = await _reservationService.BookRoomAsync(reservation);

            if (reservation == null)
                return new ReturnApi(false);
            if (reservation.payment_method == PaymentMethod.PayInHotel)
                return new ReturnApi(true, "Đặt phòng thành công", reservation.booking_id);
            if (reservation.payment_method == PaymentMethod.VNPay)
                return new ReturnApi(true, "Tạo url thanh toán thành công", _vnPayService.CreatePaymentUrl(HttpContext, new VnPaymentRequestModel
                {
                    OrderId = reservation.booking_id.ToString(),
                    FullName = reservation.first_name + " " + reservation.last_name,
                    Amount = reservation.total_price,
                    Description = "Thanh toán khách sạn.",
                    CreatedDate = DateTime.Now
                }));
            if (reservation.payment_method == PaymentMethod.Momo)
            {
                string? url = await _momoService.CreatePaymentAsync(new OrderInfoModel
                {
                    full_name = reservation.first_name + " " + reservation.last_name,
                    order_id = reservation.booking_id.ToString(),
                    amount = reservation.total_price,
                    order_info = "",
                });
                if (url != null)
                    return new ReturnApi(true, "Tạo url thanh toán thành công", url);
            }
            return new ReturnApi(false);
        }

        [JwtAuthorize]
        [HttpPut("cancel")]
        public async Task<ReturnApi> CancelRoom(Guid booking_id)
        {
            bool result = await _reservationService.CancelRoomAsync(booking_id);
            if (result)
                return new ReturnApi(true, "Hủy phòng thành công !");
            return new ReturnApi(false);
        }

        [HttpGet("{id}")]
        public async Task<ReturnApi> BookingDetail([FromRoute] Guid id)
        {
            Reservation? reservation = await _reservationService.GetReservationAsync(id);
            if (reservation == null)
                return new ReturnApi(false);
            return new ReturnApi(true, "Lấy thông tin đặt phòng thành công", reservation);
        }
    }
}