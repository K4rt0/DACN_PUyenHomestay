using Backend.Controllers;
using Backend.Models;
using Backend.Models.Enums;

namespace Backend.Services
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetReservationsAsync();
        Task<Reservation> BookRoomAsync(Reservation reservation);
        Task<bool> CancelRoomAsync(Guid bookingId);
        Task<Reservation> GetReservationAsync(Guid bookingId);
        Task<List<StatisticalBranch>> GeGetStatisticalYearAsync(int year);
        bool UpdateStatusReservation(Guid bookingId, ReservationStatus status);
        Task<Reservation> LeaveCommentAsync(Guid booking_id, int rate, string comment);
    }
}