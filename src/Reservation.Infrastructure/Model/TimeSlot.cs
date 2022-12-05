using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Infrastructure.Model
{
    public record TimeSlot(DateTimeOffset Start, int DurationMinutes)
    {
        public DateTimeOffset End => Start.AddMinutes(DurationMinutes);

        public bool Overlaps(DateTimeOffset start, int durationMinutes) => Overlaps(new TimeSlot(start, durationMinutes));

        public bool Overlaps(TimeSlot slot) => Start <= slot.End && slot.Start <= End;
    }
}
