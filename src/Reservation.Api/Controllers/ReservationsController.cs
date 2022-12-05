using Microsoft.AspNetCore.Mvc;
using Reservation.Infrastructure.Errors;
using Reservation.Infrastructure.Model;
using Reservation.Infrastructure.Repository;

namespace Reservation.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IFreeSlotRepository _freeSlotRepository;
        private readonly IReservationRepository _reservationRepository;

        public ReservationsController(IFreeSlotRepository freeSlotRepository, IReservationRepository reservationRepository)
        {
            _freeSlotRepository = freeSlotRepository;
            _reservationRepository = reservationRepository;
        }

        /// <summary>
        /// Gets the free slots. If no provider ID is specified, gets free slots for all providers.
        /// </summary>
        /// <param name="providerId">The provider ID.</param>
        /// <returns>The available slots.</returns>
        [HttpGet("free")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProviderTimeSlot>>> GetFree(string? providerId)
        {
            return Ok(await _freeSlotRepository.GetFreeSlots(providerId, DateTimeOffset.UtcNow.AddDays(1), null));
        }

        /// <summary>
        /// Gets the specified reservation.
        /// </summary>
        /// <param name="reservationId">The reservationId.</param>
        /// <returns>The reservation if it exists.</returns>
        [HttpGet("{reservationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClientReservation>> GetReservation(string reservationId)
        {
            try
            {
                return Ok(await _reservationRepository.GetReservation(reservationId));
            }
            catch (ReservationNotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        /// <summary>
        /// Confirms the specified reservation.
        /// </summary>
        /// <param name="reservationId">The reservationId.</param>
        /// <returns>200/OK if successful.</returns>
        [HttpPut("{reservationId}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ConfirmReservation(string reservationId)
        {
            try
            {
                await _reservationRepository.ConfirmReservation(reservationId);
                return Ok();
            }
            catch (ReservationNotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        /// <summary>
        /// Creates a reservation using the specified client/provider IDs and start time.
        /// </summary>
        /// <param name="clientId" example="Darth Vader">The client ID.</param>
        /// <param name="providerId" example="Sheev Palpatine">The provider ID.</param>
        /// <param name="start">The start time.</param>
        /// <returns>The new reservation.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClientReservation>> CreateReservation([FromQuery] string clientId, [FromQuery] string providerId, [FromQuery] DateTimeOffset start)
        {
            if (start < DateTimeOffset.UtcNow.AddDays(1))
            {
                return Problem("Reservations must be made at least 24 hours in advance.", statusCode: StatusCodes.Status400BadRequest);
            }

            TimeSlot slot = new TimeSlot(start, 15);
            IEnumerable<ProviderTimeSlot> free = await _freeSlotRepository.GetFreeSlots(providerId);
            if (!free.Any(x => x.Overlaps(slot)))
            {
                return Problem(detail: $"No availability for the provider [{providerId}] at [{start}]", statusCode: StatusCodes.Status400BadRequest);
            }

            try
            {
                ClientReservation reservation = await _reservationRepository.CreateReservation(clientId, providerId, new TimeSlot(start, 15));
                return CreatedAtAction(nameof(GetReservation), new { reservationId = reservation.ReservationId }, reservation);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}