namespace BirthdayRoomsBackend.Business.Bookings.Exceptions
{
    public class InvalidBookingTimeException : Exception
    {
        public InvalidBookingTimeException(string message) : base(message) { }
    }
}
