using FluentValidation;

namespace BirthdayRoomsBackend.DTOs.Validators
{
    public class BookingRequestValidator : AbstractValidator<BookingRequestDTO>
    {
        public BookingRequestValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("Customer name is required.");

            RuleFor(x => x.RoomId)
                .GreaterThan(0)
                .WithMessage("RoomId must be greater than 0.");

            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("StartTime must be before EndTime.");
        }
    }
}
