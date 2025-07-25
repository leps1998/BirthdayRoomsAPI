namespace BirthdayRoomsBackend.Business.Bookings.Exceptions
{
    public class OverlappingBookingException : Exception
    {
        public OverlappingBookingException(string message) : base(message) { }
    }
}
