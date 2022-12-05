using Reservation.Infrastructure.Model;
using System.Diagnostics.CodeAnalysis;

namespace Reservation.Infrastructure.InMemory
{
    internal class ReservationOverlapComparer : IEqualityComparer<ProviderTimeSlot>
    {
        public bool Equals(ProviderTimeSlot? x, ProviderTimeSlot? y) =>
            (x, y) switch
            {
                (null, null) => true,
                (_, null) => false,
                (null, _) => false,
                (_, _) => x.ProviderId == y.ProviderId && x.Overlaps(y)
            };

        public int GetHashCode([DisallowNull] ProviderTimeSlot obj)
        {
            return obj.GetHashCode();
        }
    }
}