using Backend.Controllers;
using Backend.DataAccess;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using DotEnv.Core;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ReservationRepository : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ReservationRepository(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<List<Reservation>> GetReservationsAsync()
        {
            var reservations = await _context.Reservations!
                                             .Include(r => r.room)
                                                .ThenInclude(r => r!.branch)
                                             .OrderBy(r => r.created_at)
                                             .ToListAsync();
            foreach (var reservation in reservations)
            {
                reservation.user = null;
            }
            return reservations;
        }

        public async Task<Reservation> BookRoomAsync(Reservation reservation)
        {
            await _context.Reservations!.AddAsync(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> CancelRoomAsync(Guid bookingId)
        {
            Reservation? reservation = await _context.Reservations!.Include(u => u.user).FirstOrDefaultAsync(r => r.booking_id == bookingId);
            if (reservation == null)
                return false;

            if (reservation.status == ReservationStatus.Cancelled || reservation.status == ReservationStatus.Failed)
                return false;

            User? user = _context.Users!.FirstOrDefault(u => u.id == reservation.user!.id);
            if (user == null)
                return false;

            user.reward_points += reservation.reward_points;
            reservation.status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();
            return reservation != null;
        }

        public async Task<Reservation> GetReservationAsync(Guid bookingId)
        {
            Reservation? reservation = await _context.Reservations!
                            .Include(r => r.room)
                            .ThenInclude(r => r!.branch)
                            .Include(r => r.room)
                            .ThenInclude(r => r!.facilities_rooms)
                            .ThenInclude(r => r.facility)
                            .Include(r => r.room!.room_images.Take(1))
                            .Include(r => r.user)
                            .FirstOrDefaultAsync(r => r.booking_id == bookingId);

            if (reservation != null && reservation.user != null)
            {
                // _context.Entry(reservation.user).State = EntityState.Detached;
                reservation.user.password = null;
            }

            return reservation ?? null!;
        }

        public bool UpdateStatusReservation(Guid bookingId, ReservationStatus status)
        {
            Reservation? reservation = _context.Reservations!.Include(u => u.user).FirstOrDefault(r => r.booking_id == bookingId);
            if (reservation == null)
                return false;

            User? user = _userService.GetByIdAsync(reservation.user!.id).Result;
            if (user == null)
                return false;

            if (status == ReservationStatus.Done)
            {
                if (reservation.status == ReservationStatus.Done)
                    return false;
                int rewardPoints = (int)(reservation.total_price / int.Parse(EnvReader.Instance["REWARD_GIFT_PER"]));
                user.reward_points += rewardPoints;
            }

            if (status == ReservationStatus.Cancelled || status == ReservationStatus.Failed)
            {
                if (reservation.status == ReservationStatus.Cancelled || reservation.status == ReservationStatus.Failed)
                    return false;
                user.reward_points += reservation.reward_points;
            }

            reservation.status = status;
            _context.SaveChanges();
            return true;
        }


        public async Task<List<StatisticalBranch>> GeGetStatisticalYearAsync(int year)
        {
            var branches = await _context.Branches!.ToListAsync();

            var branchMonthlyRevenue = await _context.Set<Reservation>()
                .Where(r => r.status == ReservationStatus.Done
                    && r.check_in.HasValue
                    && r.check_in.Value.Year == year)
                .GroupBy(r => new { r.room!.branch!.name, r.check_in!.Value.Month })
                .Select(g => new
                {
                    g.Key.name,
                    g.Key.Month,
                    TotalRevenue = g.Sum(r => r.total_price)
                })
                .ToListAsync();

            var result = branches
                .Select(branch => new StatisticalBranch
                {
                    name = branch.name,
                    data = Enumerable.Range(1, 12)
                        .Select(month => branchMonthlyRevenue
                            .FirstOrDefault(x => x.name == branch.name && x.Month == month)?.TotalRevenue ?? 0)
                        .ToList()
                })
                .ToList();

            return result;
        }

        public async Task<Reservation> LeaveCommentAsync(Guid booking_id, int rate, string comment = "")
        {
            Reservation? reservation = await _context.Reservations!.FirstOrDefaultAsync(r => r.booking_id == booking_id);
            if (reservation == null)
                return null!;

            if (!string.IsNullOrEmpty(comment))
            {
                reservation.comment = comment;
            }
            reservation.rating = rate;
            await _context.SaveChangesAsync();
            return reservation;
        }
    }
}