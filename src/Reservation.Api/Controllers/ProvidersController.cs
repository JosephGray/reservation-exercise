using Microsoft.AspNetCore.Mvc;
using Reservation.Infrastructure.Model;
using Reservation.Infrastructure.Repository;

namespace Reservation.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProvidersController : ControllerBase
    {
        private readonly IProviderRepository _providerRepository;

        public ProvidersController(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        /// <summary>
        /// Adds the specified availability to the provider.
        /// </summary>
        /// <param name="providerId" example="Sheev Palpatine">The provider ID.</param>
        /// <param name="timeSlot">The time slot.</param>
        /// <returns>200/OK on success.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddAvailability(string providerId, TimeSlot timeSlot)
        {
            await _providerRepository.UpdateSchedule(providerId, new[] { timeSlot });
            return Ok();
        }
    }
}