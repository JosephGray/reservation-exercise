using Reservation.Infrastructure.Model;

namespace Reservation.Infrastructure.Errors
{
    public class ProviderTimeSlotUnavailableException : ApplicationException
    {
        public ProviderTimeSlotUnavailableException(string providerId, TimeSlot slot)
            : base($"The time slot for provider [{providerId}] betweeen [{slot.Start}] and [{slot.End}] is unavailable.")
        {
        }
    }
}
