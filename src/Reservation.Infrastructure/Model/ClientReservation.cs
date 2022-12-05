namespace Reservation.Infrastructure.Model
{
    // it occurred to me about 90 minutes into this exercise that rather than breaking things out like I have, that I should have a provider ID and client ID on the time slot class,
    // and populate an 'available slots' collection in the backend as soon as the provider adds availability, leaving the client ID blank until a reservation is made. In keeping with
    // the spirit of the exercise, however, I'll leave it as-is.
    public record ClientReservation(string ReservationId, string ClientId, string ProviderId, bool IsConfirmed, DateTimeOffset Start, int DurationMinutes)
        : ProviderTimeSlot(ProviderId, Start, DurationMinutes)
    {
        public DateTimeOffset SubmittedTime { get; } = DateTimeOffset.UtcNow;
    };
}
