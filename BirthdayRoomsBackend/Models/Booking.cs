using System.ComponentModel.DataAnnotations;

namespace BirthdayRoomsBackend.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public Room? Room { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
