using System.Diagnostics;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/reservation")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet("get-by-id")]
        public async Task<ReturnApi> GetReservationAsync(Guid id)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            return new ReturnApi(true, "Lấy dữ liệu thành công !", await _reservationService.GetReservationAsync(id));
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpPut]
        public ReturnApi UpdateStatusReservationAsync(Guid id, string action)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (_reservationService.UpdateStatusReservation(id, action == "done" ? ReservationStatus.Done : action == "accept" ? ReservationStatus.Accepted : action == "cancel" ? ReservationStatus.Cancelled : ReservationStatus.Failed))
                return new ReturnApi(true, "Cập nhật thành công !");

            return new ReturnApi(false, "Cập nhật thất bại !");
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet("get-pagination")]
        public async Task<ReturnApi> GetReservationsPaginationAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "", [FromQuery] string filter = "")
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            if (page < 1 || pageSize < 1)
                return new ReturnApi(false, "Trang hoặc số lượng bản ghi không hợp lệ !");

            List<Reservation> reservations = await _reservationService.GetReservationsAsync();
            if (reservations.Count == 0)
                return new ReturnApi(true, "Không có chi nhánh nào !", new
                {
                    totalItems = 0,
                    totalPages = 0,
                    currentPage = 1,
                    pageSize,
                    items = new List<Branch>()
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                reservations = reservations.Where(reservation =>
                    (reservation.first_name + " " + reservation.last_name).Contains(search, StringComparison.OrdinalIgnoreCase)
                || reservation.booking_id.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                || reservation.phone!.Contains(search, StringComparison.OrdinalIgnoreCase)
                || reservation.email!.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (filter == "Đơn đặt phòng cũ nhất") reservations = reservations.OrderByDescending(reservation => reservation.created_at).ToList();
            else if (filter == "Đã huỷ") reservations = reservations.Where(reservation => reservation.status == ReservationStatus.Cancelled || reservation.status == ReservationStatus.Failed).ToList();
            else if (filter == "Đã đặt") reservations = reservations.Where(reservation => reservation.status == ReservationStatus.Booked).ToList();
            else if (filter == "Chờ xác nhận") reservations = reservations.Where(reservation => reservation.status == ReservationStatus.Pending).ToList();
            else if (filter == "Đã xác nhận") reservations = reservations.Where(reservation => reservation.status == ReservationStatus.Accepted).ToList();
            else if (filter == "Đã xong") reservations = reservations.Where(reservation => reservation.status == ReservationStatus.Done).ToList();

            int totalItems = reservations.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            List<Reservation> reservationsInPage = reservations
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            return new ReturnApi(true, "Lấy dữ liệu thành công !", new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize,
                items = reservationsInPage
            });
        }

        [JwtAuthorize(UserRole.Staff)]
        [HttpGet("statistical")]
        public async Task<ReturnApi> GetStatisticalAsync(int year)
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            var result = await _reservationService.GeGetStatisticalYearAsync(year);
            return new ReturnApi(true, "Lấy dữ liệu thành công", result);
        }

        [JwtAuthorize]
        [HttpPut("leave-comment")]
        public async Task<ReturnApi> LeaveCommentAsync(Guid booking_id, int rate, string comment = "")
        {
            if (!ModelState.IsValid)
                return new ReturnApi(false);

            Reservation? reservation = await _reservationService.LeaveCommentAsync(booking_id, rate, comment);
            if (reservation != null)
                return new ReturnApi(true, "Cập nhật thành công !");

            return new ReturnApi(false, "Cập nhật thất bại !");
        }
    }

    public class StatisticalBranch
    {
        public string? name { get; set; }
        public List<double>? data { get; set; }
    }
}