using Reservation.Infrastructure.Model;
using Reservation.Infrastructure.Repository;

namespace Reservation.Infrastructure.InMemory
{
    public class FreeSlotRepository : IFreeSlotRepository
    {
        private static readonly int slotDurationMinutes = 15;
        private readonly IEqualityComparer<ProviderTimeSlot> _comparer = new ReservationOverlapComparer();
        private readonly IProviderRepository _providerRepository;
        private readonly IReservationRepository _reservationRepository;

        public FreeSlotRepository(IProviderRepository providerRepository, IReservationRepository reservationRepository)
        {
            _providerRepository = providerRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<ProviderTimeSlot>> GetFreeSlots(string? providerId, DateTimeOffset? start, DateTimeOffset? stop)
        {
            IEnumerable<ProviderTimeSlot> providerSchedules = (await _providerRepository.GetAvailability(providerId, start, stop)).SelectMany(x => Split(x)).ToList();
            IEnumerable<ProviderTimeSlot> reservedTimes = (await _reservationRepository.GetReservations(null, providerId, start, stop, null));

            if (!reservedTimes.Any())
            {
                return providerSchedules.AsEnumerable();
            }

            IEnumerable<ProviderTimeSlot> free =
                from p in providerSchedules
                from r in reservedTimes
                where !p.Overlaps(r)
                select p;

            return free.ToList().AsEnumerable();
        }

        private IEnumerable<ProviderTimeSlot> Split(ProviderTimeSlot slot)
        {
            int chunks = (int)slot.DurationMinutes / slotDurationMinutes;
            for (int i = 0; i < chunks; i++)
            {
                yield return new ProviderTimeSlot(slot.ProviderId, slot.Start.AddMinutes(i * slotDurationMinutes), slotDurationMinutes);
            }
        }

    }
}