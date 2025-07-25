namespace BirthdayRoomsBackend.DTOs
{
    public class BookingRequestDTO
    {
        public int RoomId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
