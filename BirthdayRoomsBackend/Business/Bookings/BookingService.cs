using BirthdayRoomsBackend.Business.Bookings.Exceptions;
using BirthdayRoomsBackend.Data;
using BirthdayRoomsBackend.DTOs;
using BirthdayRoomsBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BirthdayRoomsBackend.Business.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateBookingAsync(BookingRequestDTO request)
        {
            BookingRules.ValidateBookingTime(
                request.Date.Add(request.StartTime),
                request.Date.Add(request.EndTime));

            var existingBookings = await _context.Bookings
                .Where(b => b.RoomId == request.RoomId && b.Date.Date == request.Date.Date)
                .ToListAsync();

            foreach (var existing in existingBookings)
            {
                if (BookingRules.OverlapsWithBuffer(request, existing))
                    throw new OverlappingBookingException("Booking overlaps with another or violates buffer time.");
            }

            var booking = new Booking
            {
                RoomId = request.RoomId,
                CustomerName = request.CustomerName,
                Date = request.Date.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (booking == null)
                return false;

            booking.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Where(b => !b.IsDeleted)
                .Include(b => b.Room)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _context.Bookings
                .Where(b => b.Date.Date == date.Date)
                .Include(b => b.Room)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Where(b => !b.IsDeleted && b.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Booking> UpdateAsync(int id, BookingRequestDTO request)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found or has been deleted.");

            BookingRules.ValidateBookingTime(
                request.Date.Add(request.StartTime),
                request.Date.Add(request.EndTime));

            var overlapping = await _context.Bookings
                .Where(b => b.RoomId == request.RoomId &&
                            b.Date.Date == request.Date.Date &&
                            b.Id != id &&
                            !b.IsDeleted)
                .ToListAsync();

            if (overlapping.Any(b => BookingRules.OverlapsWithBuffer(request, b)))
                throw new OverlappingBookingException("Booking overlaps with another or violates buffer time.");

            booking.RoomId = request.RoomId;
            booking.CustomerName = request.CustomerName;
            booking.Date = request.Date;
            booking.StartTime = request.StartTime;
            booking.EndTime = request.EndTime;

            await _context.SaveChangesAsync();

            return booking;
        }
    }
}
