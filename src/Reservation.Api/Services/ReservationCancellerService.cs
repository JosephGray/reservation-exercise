using Reservation.Infrastructure.Model;
using Reservation.Infrastructure.Repository;
using System.Runtime.CompilerServices;

namespace Reservation.Api.Services
{
    // this could be unnecessary depending on the backend; using mongo, we can place tentative reservations in a collection with a TTL index that will automatically delete the
    // record after a set amount of time; confirmed reservations would exist in a different table that does not have a TTL index.
    public class ReservationCancellerService : IHostedService
    {
        private readonly TimeSpan MaxUnconfirmedReservationAge = TimeSpan.FromMinutes(30);
        private readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IReservationRepository _reservationRepository;
        private Task _loop = Task.CompletedTask;

        public ReservationCancellerService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _loop = CancelExpiredReservations();
            return Task.CompletedTask;
        }

        private async Task CancelExpiredReservations()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                IEnumerable<Task> expired = (await _reservationRepository.GetReservations(isConfirmed: false)).Where(x => DateTimeOffset.UtcNow > x.SubmittedTime + MaxUnconfirmedReservationAge).Select(x => _reservationRepository.CancelReservation(x.ReservationId));
                await Task.WhenAll(expired);
                await Task.Delay(PollingInterval);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return _loop;
        }
    }
}
