using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Api.ViewModels;
using ESP.FlightBook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESP.FlightBook.Api.Controllers.v1
{
    public class ApproachesController : LogbookApiController
    {
        protected readonly ILogger<ApproachesController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public ApproachesController(ApplicationDbContext apiDbContext, ILogger<ApproachesController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Approaches resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (401) Unauthorized - The request is not authorized
        /// (404) Not Found - The associated flight is not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches", Name = "GetAllApproachesRoute")]
        public async Task<IActionResult> GetAllApproaches(int logbookId, int flightId)
        {
            // Find associated flight
            Flight flight = await _apiDbContext.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FlightId == flightId && f.LogbookId == logbookId);

            // Return not found result
            if (flight == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = flight.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Find specified resources
            List<Approach> approachList = await _apiDbContext.Approaches
                .AsNoTracking()
                .Where(a => a.FlightId == flightId && a.Flight.LogbookId == logbookId)
                .ToListAsync();

            // Convert to DTO
            List<ApproachDTO> dtoList = new List<ApproachDTO>();
            foreach (Approach approach in approachList)
            {
                dtoList.Add(ApproachDTO.ToDto(approach));
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Approach resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="approachId">Unique approach identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}", Name = "GetApproachByIdRoute")]
        public async Task<IActionResult> GetApproach(int logbookId, int flightId, int approachId)
        {
            // Find specified resource
            Approach approach = await _apiDbContext.Approaches
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApproachId == approachId && a.FlightId == flightId && a.Flight.LogbookId == logbookId);

            // Return not found result
            if (approach == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = approach.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            ApproachDTO dto = ApproachDTO.ToDto(approach);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Approach resource for the specified Flight
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="approach">The approach to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches", Name = "PostApproachRoute")]
        public async Task<IActionResult> PostApproach(int logbookId, int flightId, [FromBody] Approach approach)
        {
            // Validate request data
            if (approach == null)
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
                        _logger.LogInformation("[PostApproach] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find associated flight
            Flight flight = await _apiDbContext.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FlightId == flightId && f.LogbookId == logbookId);

            // Return not found result
            if (flight == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = flight.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            approach.FlightId = flightId;
            approach.UserId = GetUserIdClaim();
            approach.CreatedOn = DateTime.UtcNow;
            approach.ChangedOn = approach.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Approaches.Add(approach);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostApproach]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            ApproachDTO dto = ApproachDTO.ToDto(approach);

            // Return success
            return CreatedAtRoute("GetApproachByIdRoute",
                new
                {
                    logbookId = logbookId,
                    flightId = dto.FlightId,
                    approachId = dto.ApproachId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Approach resource for the specified Flight
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="approachId">Unique approach identifier</param>
        /// <param name="approach">Approach to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}", Name = "PutApproachRoute")]
        public async Task<IActionResult> PutApproach(int logbookId, int flightId, int approachId, [FromBody] Approach approach)
        {
            // Validate request data
            if (approach == null)
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
                        _logger.LogInformation("[PutApproach] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Approach existingApproach = await _apiDbContext.Approaches
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ApproachId == approachId && a.FlightId == flightId && a.Flight.LogbookId == logbookId);

            // Return not found result
            if (existingApproach == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingApproach.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            approach.ApproachId = existingApproach.ApproachId;
            approach.FlightId = existingApproach.FlightId;
            approach.UserId = existingApproach.UserId;
            approach.CreatedOn = existingApproach.CreatedOn;
            approach.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(approach).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutApproach]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Approach resource for the specified Flight
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="flightId">Unique flight identifier</param>
        /// <param name="approachId">Unique approach identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}", Name = "DeleteApproachRoute")]
        public async Task<IActionResult> DeleteApproach(int logbookId, int flightId, int approachId)
        {
            // Check to see if the resource exists
            Approach existingApproach = await _apiDbContext.Approaches
                .FirstOrDefaultAsync(a => a.ApproachId == approachId && a.FlightId == flightId && a.Flight.LogbookId == logbookId);

            // Return not found result
            if (existingApproach == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingApproach.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Approaches.Remove(existingApproach);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteApproach]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            ApproachDTO dto = ApproachDTO.ToDto(existingApproach);

            // Return result
            return Ok(dto);
        }
    }
}
