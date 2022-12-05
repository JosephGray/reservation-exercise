using Reservation.Infrastructure.Model;
using Swashbuckle.AspNetCore.Filters;

namespace Reservation.Api.Example
{
    public class TimeSlotExampleProvider : IExamplesProvider<TimeSlot>
    {
        public TimeSlot GetExamples() => new TimeSlot(DateTimeOffset.UtcNow.AddHours(1), 15);
    }

    public class DateTimeOffsetExampleProvider : IExamplesProvider<DateTimeOffset>
    {
        public DateTimeOffset GetExamples()
        {
            return DateTimeOffset.UtcNow.AddHours(1);
        }
    }
}
