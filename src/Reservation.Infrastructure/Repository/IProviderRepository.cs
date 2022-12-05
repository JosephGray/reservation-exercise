using Reservation.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Infrastructure.Repository
{
    public interface IProviderRepository
    {
        Task UpdateSchedule(string providerId, IEnumerable<TimeSlot> availability);
        Task<IEnumerable<ProviderTimeSlot>> GetAvailability(string? providerId, DateTimeOffset? start, DateTimeOffset? stop);
    }
}
