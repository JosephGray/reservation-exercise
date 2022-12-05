using Reservation.Infrastructure.Model;

namespace Reservation.Infrastructure.Repository
{
    public interface IFreeSlotRepository
    {
        Task<IEnumerable<ProviderTimeSlot>> GetFreeSlots(string? providerId = null, DateTimeOffset? start = null, DateTimeOffset? stop = null);
    }
}
