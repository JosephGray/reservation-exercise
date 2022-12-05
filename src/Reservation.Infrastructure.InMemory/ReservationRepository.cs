using Reservation.Infrastructure.Errors;
using Reservation.Infrastructure.Model;
using Reservation.Infrastructure.Repository;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Reservation.Infrastructure.InMemory
{
    public class ReservationRepository : IReservationRepository
    {
        private int _reservationId = 0;
        private readonly ReservationOverlapComparer _comparer = new ReservationOverlapComparer();
        private readonly ConcurrentDictionary<string, ClientReservation> _reservations = new ConcurrentDictionary<string, ClientReservation>();
        private readonly HashidsNet.Hashids _hasher = new HashidsNet.Hashids(minHashLength: 6);

        public Task CancelReservation(string reservationId)
        {
            if (!_reservations.Remove(reservationId, out _))
            {
                throw new ReservationNotFoundException(reservationId);
            }

            return Task.CompletedTask;
        }

        public Task ConfirmReservation(string reservationId)
        {
            ClientReservation reservation = GetReservationCore(reservationId);
            _reservations[reservationId] = reservation with { IsConfirmed = true };
            return Task.CompletedTask;
        }

        public Task<ClientReservation> CreateReservation(string clientId, string providerId, TimeSlot slot)
        {
            int id = Interlocked.Increment(ref _reservationId);
            ClientReservation reservation = new ClientReservation(_hasher.Encode(id), clientId, providerId, false, slot.Start, slot.DurationMinutes);
            
            // a real reservation system should validate that we are not overbooking a provider for the specified slot; this could be done in mongo by creating a unique
            // compound key to include the provider ID and start time if we are always locked into durations of 15 minutes. The best way to handle it (especially if
            // durations are variable) would be to use kafka and partition messages by provider ID so that scheduling requests are serialized per provider.
            lock (_reservations)
            {
                if (_reservations.Values.Contains(reservation, _comparer))
                {
                    throw new ProviderTimeSlotUnavailableException(providerId, slot);
                }

                _reservations.TryAdd(reservation.ReservationId, reservation);
            }

            return Task.FromResult(reservation);
        }

        public Task<ClientReservation> GetReservation(string reservationId) => Task.FromResult(GetReservationCore(reservationId));

        public Task<IEnumerable<ClientReservation>> GetReservations(string? clientId, string? providerId, DateTimeOffset? start, DateTimeOffset? stop, bool? isConfirmed)
        {
            IEnumerable<ClientReservation> reservations = _reservations.Values;

            if (!string.IsNullOrEmpty(clientId))
            {
                reservations = reservations.Where(x => x.ClientId == clientId);
            }

            if (!string.IsNullOrEmpty(providerId))
            {
                reservations = reservations.Where(x => x.ProviderId == providerId);
            }

            if (start.HasValue)
            {
                reservations = reservations.Where(x => x.Start >= start);
            }

            if (stop.HasValue)
            {
                reservations = reservations.Where(x => x.End <= stop);
            }

            if (isConfirmed.HasValue)
            {
                reservations = reservations.Where(x => x.IsConfirmed == isConfirmed);
            }

            return Task.FromResult(reservations.ToList().AsEnumerable());
        }

        private ClientReservation GetReservationCore(string reservationId)
        {
            if (!_reservations.TryGetValue(reservationId, out ClientReservation? reservation))
            {
                throw new ReservationNotFoundException(reservationId);
            }

            return reservation;
        }
    }
}