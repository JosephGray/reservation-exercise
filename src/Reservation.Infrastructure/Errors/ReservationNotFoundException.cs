using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Infrastructure.Errors
{
    public class ReservationNotFoundException : ApplicationException
    {
        public ReservationNotFoundException(string reservationId)
            : base($"The reservation [{reservationId}] was not found.")
        {
        }
    }
}
