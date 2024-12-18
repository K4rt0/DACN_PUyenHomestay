using System.Diagnostics;
using Backend.DataAccess;
using Backend.Extensions;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PaymentController(IVnPayService vnPayService, IMomoService momoService, ApplicationDbContext context, IEmailService emailService)
        {
            _emailService = emailService;
            _context = context;
            _vnPayService = vnPayService;
            _momoService = momoService;
        }
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            string? bid = response.OrderDescription;
            string? emailBody = "";

            Reservation? reservation = await _context.Reservations!
                .Include(p => p.user)
                .Include(p => p.room)
                    .ThenInclude(p => p!.branch)
                .Include(p => p.room)
                    .ThenInclude(p => p!.room_images)
                .FirstOrDefaultAsync(r => r.booking_id.ToString() == bid);

            if (reservation == null)
                return Redirect($"http://localhost:5173/payment-return?paymentType=vnpay&paymentSuccess=false&bid={bid}");
            if (response.VnPayResponseCode != "00")
            {
                emailBody = SendEmail.NotifyBookingTemplate(false, reservation);
                await _emailService.SendEmailAsync(reservation?.user?.email!, "Thông tin chi tiết đơn đặt phòng", emailBody!);

                reservation!.status = ReservationStatus.Failed;
                reservation.user!.reward_points += reservation.reward_points;
                await _context.SaveChangesAsync();
                return Redirect("http://localhost:5173/payment-return?paymentType=vnpay&paymentSuccess=false");
            }

            reservation.status = response.VnPayResponseCode == "00" ? ReservationStatus.Booked : ReservationStatus.Failed;
            reservation.payment_status = true;
            await _context.SaveChangesAsync();

            emailBody = SendEmail.NotifyBookingTemplate(true, reservation);
            await _emailService.SendEmailAsync(reservation?.user?.email!, "Thông tin chi tiết đơn đặt phòng", emailBody!);

            return Redirect($"http://localhost:5173/payment-return?paymentType=vnpay&paymentSuccess={response.VnPayResponseCode == "00"}&bid={bid}");
        }
        [HttpGet("momo-return")]
        public async Task<IActionResult> MomoReturn()
        {
            var response = _momoService.PaymentExecuteAsync(Request.Query);
            string? bid = response.OrderId;
            string? emailBody = "";
            Reservation? reservation = await _context.Reservations!.FirstOrDefaultAsync(r => r.booking_id.ToString() == bid);

            if (reservation == null)
                return Redirect($"http://localhost:5173/payment-return?paymentType=momo&paymentSuccess=false&bid={bid}");

            if (response.errorCode != 0)
            {
                emailBody = SendEmail.NotifyBookingTemplate(false, reservation);
                await _emailService.SendEmailAsync(reservation?.user?.email!, "Thông tin chi tiết đơn đặt phòng", emailBody!);

                reservation!.status = ReservationStatus.Failed;
                await _context.SaveChangesAsync();
                return Redirect("http://localhost:5173/payment-return?paymentType=momo&paymentSuccess=false");
            }

            reservation.status = response.errorCode == 0 ? ReservationStatus.Booked : ReservationStatus.Failed;
            reservation.payment_status = true;
            await _context.SaveChangesAsync();

            emailBody = SendEmail.NotifyBookingTemplate(true, reservation);
            await _emailService.SendEmailAsync(reservation?.user?.email!, "Thông tin chi tiết đơn đặt phòng", emailBody!);

            return Redirect($"http://localhost:5173/payment-return?paymentType=momo&paymentSuccess={response.errorCode == 0}&bid={bid}");
        }
    }
}