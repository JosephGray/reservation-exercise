namespace Reservation.Infrastructure.Model
{
    public record ProviderSchedule(string ProviderId, IEnumerable<ProviderTimeSlot> ScheduledHours);
}
