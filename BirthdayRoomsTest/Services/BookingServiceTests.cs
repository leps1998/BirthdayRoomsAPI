using BirthdayRoomsBackend.Business.Bookings;
using BirthdayRoomsBackend.Business.Bookings.Exceptions;
using BirthdayRoomsBackend.Data;
using BirthdayRoomsBackend.DTOs;
using BirthdayRoomsBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BirthdayRoomsTest.Services
{
    public class BookingServiceTests
    {
        private AppDbContext GetAppDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Rooms.Add(new Room { Id = 1, Name = "Room A" });
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task CreateBookingShouldSucceedWhenValid()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);

            var request = new BookingRequestDTO
            {
                RoomId = 1,
                CustomerName = "Leonel",
                Date = DateTime.Today.AddDays(1),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0)
            };

            var result = await service.CreateBookingAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.RoomId, result.RoomId);
            Assert.Equal(request.CustomerName, result.CustomerName);
        }

        [Fact]
        public async Task CreateBooking_ShouldThrow_WhenOverlapsWithExisting()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            context.Bookings.Add(new Booking
            {
                RoomId = 1,
                CustomerName = "Juan",
                Date = date,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0)
            });
            await context.SaveChangesAsync();

            var request = new BookingRequestDTO
            {
                RoomId = 1,
                CustomerName = "Leo",
                Date = date,
                StartTime = new TimeSpan(11, 15, 0),
                EndTime = new TimeSpan(12, 0, 0)
            };

            await Assert.ThrowsAsync<OverlappingBookingException>(() => service.CreateBookingAsync(request));
        }

        [Fact]
        public async Task CreateBooking_ShouldThrow_WhenOutsideWorkingHours()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            var request = new BookingRequestDTO
            {
                RoomId = 1,
                CustomerName = "Leo",
                Date = date,
                StartTime = new TimeSpan(8, 0, 0), 
                EndTime = new TimeSpan(10, 0, 0)
            };

            await Assert.ThrowsAsync<InvalidBookingTimeException>(() => service.CreateBookingAsync(request));
        }

        [Fact]
        public async Task CreateBooking_ShouldThrow_WhenEndTimeBeforeStartTime()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            var request = new BookingRequestDTO
            {
                RoomId = 1,
                CustomerName = "Leo",
                Date = date,
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(13, 0, 0)
            };

            await Assert.ThrowsAsync<InvalidBookingTimeException>(() => service.CreateBookingAsync(request));
        }

        [Fact]
        public async Task CreateBooking_ShouldThrow_WhenDurationIsTooShort()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            var request = new BookingRequestDTO
            {
                RoomId = 1,
                CustomerName = "Leo",
                Date = date,
                StartTime = new TimeSpan(15, 0, 0),
                EndTime = new TimeSpan(15, 20, 0) 
            };

            await Assert.ThrowsAsync<BookingDurationException>(() => service.CreateBookingAsync(request));
        }

        [Fact]
        public async Task GetAll_ShouldReturnOnlyNonDeletedBookings()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            context.Bookings.AddRange(
                new Booking
                {
                    RoomId = 1,
                    CustomerName = "Activo",
                    Date = date,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    IsDeleted = false
                },
                new Booking
                {
                    RoomId = 1,
                    CustomerName = "Eliminado",
                    Date = date,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0),
                    IsDeleted = true
                });

            await context.SaveChangesAsync();

            var result = await service.GetAllAsync();

            Assert.Single(result); 
            Assert.Equal("Activo", result[0].CustomerName);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_IfDeleted()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            var booking = new Booking
            {
                RoomId = 1,
                CustomerName = "Eliminado",
                Date = date,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                IsDeleted = true
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var result = await service.GetByIdAsync(booking.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_ShouldSetIsDeletedTrue()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            var booking = new Booking
            {
                RoomId = 1,
                CustomerName = "Eliminar",
                Date = date,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0)
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(booking.Id);

            Assert.True(result);

            var deleted = await context.Bookings.FindAsync(booking.Id);
            Assert.True(deleted?.IsDeleted);
        }

        [Fact]
        public async Task Update_ShouldThrow_IfDeleted()
        {
            var context = GetAppDbContext();
            var service = new BookingService(context);
            var date = DateTime.Today.AddDays(1);

            var booking = new Booking
            {
                RoomId = 1,
                CustomerName = "Deleted",
                Date = date,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                IsDeleted = true
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var update = new BookingRequestDTO
            {
                RoomId = 1,
                CustomerName = "Intento de cambio",
                Date = date,
                StartTime = new TimeSpan(12, 0, 0),
                EndTime = new TimeSpan(13, 0, 0)
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(booking.Id, update));
        }
    }
}
