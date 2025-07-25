using BirthdayRoomsBackend.Business.Bookings;
using BirthdayRoomsBackend.Business.Bookings.Exceptions;
using BirthdayRoomsBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BirthdayRoomsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Create a booking
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingRequestDTO request)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidBookingTimeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BookingDurationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OverlappingBookingException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        /// <summary>
        /// Get booking by id
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        /// <summary>
        /// Get bookings by date (yyyy-MM-dd)
        /// </summary>
        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Invalid date format. Expected yyyy-MM-dd.");

            var bookings = await _bookingService.GetBookingsByDateAsync(parsedDate);
            return Ok(bookings);
        }

        /// <summary>
        /// Update a booking
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingRequestDTO request)
        {
            try
            {
                var updated = await _bookingService.UpdateAsync(id, request);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidBookingTimeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BookingDurationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (OverlappingBookingException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Delete a booking
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bookingService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
