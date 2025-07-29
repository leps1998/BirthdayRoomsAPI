using BirthdayRoomsBackend.Business.Bookings.Exceptions;
using BirthdayRoomsBackend.DTOs;
using BirthdayRoomsBackend.Models;

namespace BirthdayRoomsBackend.Business.Bookings
{
    public static class BookingRules
    {
        public static readonly TimeSpan OpeningTime = TimeSpan.FromHours(9);
        public static readonly TimeSpan ClosingTime = TimeSpan.FromHours(18);
        public static readonly TimeSpan BufferTime = TimeSpan.FromMinutes(30);

        public static void ValidateBookingTime(DateTime start, DateTime end)
        {
            if (start.TimeOfDay < OpeningTime || end.TimeOfDay > ClosingTime)
                throw new InvalidBookingTimeException("Booking must be within opening hours (9:00 - 18:00).");

            if (end <= start)
                throw new InvalidBookingTimeException("End time must be after start time.");

            if ((end - start) < BufferTime)
                throw new BookingDurationException($"Booking duration must be at least {BufferTime.TotalMinutes} minutes.");
        }

        public static bool OverlapsWithBuffer(BookingRequestDTO request, Booking existing)
        {
            var requestedStart = request.Date.Add(request.StartTime);
            var requestedEnd = request.Date.Add(request.EndTime);

            var existingStart = existing.Date.Add(existing.StartTime).AddMinutes(-BufferTime.TotalMinutes);
            var existingEnd = existing.Date.Add(existing.EndTime).AddMinutes(BufferTime.TotalMinutes);

            return requestedStart < existingEnd && requestedEnd > existingStart;
        }

    }
}
