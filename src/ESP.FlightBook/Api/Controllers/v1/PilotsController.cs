using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Api.ViewModels;
using ESP.FlightBook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ESP.FlightBook.Api.Controllers.v1
{
    public class PilotsController : LogbookApiController
    {
        protected readonly ILogger<PilotsController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public PilotsController(ApplicationDbContext apiDbContext, ILogger<PilotsController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Pilot resource
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/pilots</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/pilots", Name = "GetPilotByUserIdRoute")]
        public async Task<IActionResult> GetPilot(int logbookId)
        {
            // Find specified resources
            Pilot pilot = await _apiDbContext.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.LogbookId == logbookId);

            // Return not found result
            if (pilot == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = pilot.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            PilotDTO dto = PilotDTO.ToDto(pilot);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Returns specified Pilot resource
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="pilotId">Unique pilot identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}", Name = "GetPilotByIdRoute")]
        public async Task<IActionResult> GetPilot(int logbookId, int pilotId)
        {
            // Find specified resources
            Pilot pilot = await _apiDbContext.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PilotId == pilotId && p.LogbookId == logbookId);

            // Return not found result
            if (pilot == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = pilot.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            PilotDTO dto = PilotDTO.ToDto(pilot);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Pilot resource for the specified logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="pilot">The pilot to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not Found - If the associated logbook is not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/pilots</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/pilots", Name = "PostPilotRoute")]
        public async Task<IActionResult> PostPilot(int logbookId, [FromBody] Pilot pilot)
        {
            // Validate request data
            if (pilot == null)
            {
                return BadRequest();
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var message in error.Errors)
                    {
                        _logger.LogInformation("[PostPilot] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find associated logbook
            Logbook logbook = await _apiDbContext.Logbooks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (logbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = logbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            pilot.LogbookId = logbookId;
            pilot.UserId = GetUserIdClaim();
            pilot.CreatedOn = DateTime.UtcNow;
            pilot.ChangedOn = pilot.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Pilots.Add(pilot);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostPilot]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            PilotDTO dto = PilotDTO.ToDto(pilot);

            // Return success
            return CreatedAtRoute("GetPilotByIdRoute",
                new
                {
                    logbookId = dto.LogbookId,
                    pilotId = dto.PilotId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Pilot resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="pilotId">Unique pilot identifier</param>
        /// <param name="pilot">Pilot to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}", Name = "PutPilotRoute")]
        public async Task<IActionResult> PutPilot(int logbookId, int pilotId, [FromBody] Pilot pilot)
        {
            // Validate request data
            if (pilot == null)
            {
                return BadRequest();
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var message in error.Errors)
                    {
                        _logger.LogInformation("[PutPilot] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Pilot existingPilot = await _apiDbContext.Pilots
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PilotId == pilotId && p.LogbookId == logbookId);

            // Return not found result
            if (existingPilot == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingPilot.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            pilot.PilotId = existingPilot.PilotId;
            pilot.LogbookId = existingPilot.LogbookId;
            pilot.UserId = existingPilot.UserId;
            pilot.CreatedOn = existingPilot.CreatedOn;
            pilot.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(pilot).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutPilot]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Pilot resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="pilotId">Unique pilot identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}", Name = "DeletePilotRoute")]
        public async Task<IActionResult> DeletePilot(int logbookId, int pilotId)
        {
            // Check to see if the resource exists
            Pilot existingPilot = await _apiDbContext.Pilots
                .FirstOrDefaultAsync(p => p.PilotId == pilotId && p.LogbookId == logbookId);

            // Return not found result
            if (existingPilot == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingPilot.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Pilots.Remove(existingPilot);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeletePilot]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            PilotDTO dto = PilotDTO.ToDto(existingPilot);

            // Return result
            return Ok(dto);
        }
    }
}
