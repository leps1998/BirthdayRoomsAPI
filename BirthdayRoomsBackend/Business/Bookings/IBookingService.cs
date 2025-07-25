using BirthdayRoomsBackend.DTOs;
using BirthdayRoomsBackend.Models;

namespace BirthdayRoomsBackend.Business.Bookings
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(BookingRequestDTO request);
        Task<List<Booking>> GetBookingsByDateAsync(DateTime date);
        Task<List<Booking>> GetAllAsync(); 
        Task<Booking?> GetByIdAsync(int id); 
        Task<Booking> UpdateAsync(int id, BookingRequestDTO request); 
        Task<bool> DeleteAsync(int id); 
    }
}
