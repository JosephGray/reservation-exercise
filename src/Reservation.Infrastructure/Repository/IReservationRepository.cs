using Reservation.Infrastructure.Model;

namespace Reservation.Infrastructure.Repository
{
    public interface IReservationRepository
    {
        Task<ClientReservation> CreateReservation(string clientId, string providerId, TimeSlot slot);
        Task ConfirmReservation(string reservationId);
        Task<IEnumerable<ClientReservation>> GetReservations(string? clientId = null, string? providerId = null, DateTimeOffset? start = null, DateTimeOffset? stop = null, bool? isConfirmed = null);
        Task<ClientReservation> GetReservation(string reservationId);
        Task CancelReservation(string reservationId);
    }
}
