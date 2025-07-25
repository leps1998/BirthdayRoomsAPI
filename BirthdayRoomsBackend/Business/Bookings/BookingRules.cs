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
            var rangeStart = existing.StartTime - BufferTime;
            var rangeEnd = existing.EndTime + BufferTime;

            return request.StartTime < rangeEnd && request.EndTime > rangeStart;
        }

    }
}
