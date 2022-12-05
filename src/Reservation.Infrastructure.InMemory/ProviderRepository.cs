using Reservation.Infrastructure.Model;
using Reservation.Infrastructure.Repository;
using System.Collections.Concurrent;

namespace Reservation.Infrastructure.InMemory
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly ConcurrentDictionary<string, HashSet<ProviderTimeSlot>> _schedulesByProvider = new ConcurrentDictionary<string, HashSet<ProviderTimeSlot>>();

        public Task<IEnumerable<ProviderTimeSlot>> GetAvailability(string? providerId, DateTimeOffset? start, DateTimeOffset? stop)
        {
            IEnumerable<ProviderTimeSlot> availability =
                string.IsNullOrEmpty(providerId)
                    ? _schedulesByProvider.Values.SelectMany(x => x)
                    : _schedulesByProvider.GetValueOrDefault(providerId, new HashSet<ProviderTimeSlot>());
            
            if (start.HasValue)
            {
                availability = availability.Where(x => x.Start >= start);
            }

            if (stop.HasValue)
            {
                availability = availability.Where(x => x.End <= stop);
            }

            return Task.FromResult(availability.ToList().AsEnumerable());
        }

        public Task UpdateSchedule(string providerId, IEnumerable<TimeSlot> availability)
        {
            HashSet<ProviderTimeSlot> providerAvailability = _schedulesByProvider.GetOrAdd(providerId, x => new HashSet<ProviderTimeSlot>());

            foreach (TimeSlot t in availability)
            {
                providerAvailability.Add(new ProviderTimeSlot(providerId, t.Start, t.DurationMinutes));
            }

            return Task.CompletedTask;
        }
    }
}