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
    public class AircraftController : LogbookApiController
    {
        protected readonly ILogger<AircraftController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public AircraftController(ApplicationDbContext apiDbContext, ILogger<AircraftController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Aircraft resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="page">Returns results from the given page</param>
        /// <param name="itemsPerPage">Determines the number of items per page</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/aircraft<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/aircraft", Name = "GetAllAircraftRoute")]
        public async Task<IActionResult> GetAllAircraft(int logbookId, int page = 0, int itemsPerPage = 0)
        {
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

            // Determine how many matching rows we expect
            int rowCount = await _apiDbContext.Aircraft.CountAsync(a => a.LogbookId == logbookId);
            Response.Headers.Add("X-eFlightBook-Pagination-Total", rowCount.ToString());
            Response.Headers.Add("X-eFlightBook-Pagination-Limit", itemsPerPage.ToString());

            // Create a list to store items retrieved
            List<Aircraft> aircraftList = new List<Aircraft>();

            // Get subset of items if the itemsPerPage value is more than 0,
            // otherwise get all items
            if (rowCount > 0)
            {
                if (itemsPerPage > 0)
                {
                    // Calculate number of pages needed to support the requested items per page and then
                    // adjust the requested page number to align to the total pages available
                    int totalPages = TotalPageCount(rowCount, itemsPerPage);
                    page = GetBestPage(totalPages, page);
                    Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", totalPages.ToString());
                    Response.Headers.Add("X-eFlightBook-Pagination-Page", page.ToString());

                    // Retrieve the requested items
                    aircraftList = await _apiDbContext.Aircraft
                        .AsNoTracking()
                        .Where(a => a.LogbookId == logbookId)
                        .OrderByDescending(a => a.AircraftIdentifier)
                        .Skip(page * itemsPerPage)
                        .Take(itemsPerPage)
                        .ToListAsync();
                }
                else
                {
                    // Retrieve all items
                    aircraftList = await _apiDbContext.Aircraft
                        .AsNoTracking()
                        .Where(a => a.LogbookId == logbookId)
                        .OrderByDescending(a => a.AircraftIdentifier)
                        .ToListAsync();
                    Response.Headers.Add("X-eFlightBook-Pagination-TotalPages", "1");
                    Response.Headers.Add("X-eFlightBook-Pagination-Page", "0");

                }
            }

            // Convert to DTO
            List<AircraftDTO> dtoList = new List<AircraftDTO>();
            foreach (Aircraft aircraft in aircraftList)
            {
                dtoList.Add(AircraftDTO.ToDto(aircraft));
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Aircraft resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="aircraftId">Unique aircraft identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}", Name = "GetAircraftByIdRoute")]
        public async Task<IActionResult> GetAircraft(int logbookId, int aircraftId)
        {
            // Find specified resources
            Aircraft aircraft = await _apiDbContext.Aircraft
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AircraftId == aircraftId && a.LogbookId == logbookId);

            // Return not found result
            if (aircraft == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = aircraft.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            AircraftDTO dto = AircraftDTO.ToDto(aircraft);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Aircraft resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="aircraft">The flight to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (401) Unauthorized - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/aircraft</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/aircraft", Name = "PostAircraftRoute")]
        public async Task<IActionResult> PostAircraft(int logbookId, [FromBody] Aircraft aircraft)
        {
            // Validate request data
            if (aircraft == null)
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
                        _logger.LogInformation("[PostAircraft] {0}", message.ErrorMessage);
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
            aircraft.LogbookId = logbookId;
            aircraft.UserId = GetUserIdClaim();
            aircraft.CreatedOn = DateTime.UtcNow;
            aircraft.ChangedOn = aircraft.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Aircraft.Add(aircraft);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostAircraft]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            AircraftDTO dto = AircraftDTO.ToDto(aircraft);

            // Return success
            return CreatedAtRoute("GetAircraftByIdRoute",
                new
                {
                    logbookId = dto.LogbookId,
                    aircraftId = dto.AircraftId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Aircraft resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="aircraftId">Unique aircraft identifier</param>
        /// <param name="aircraft">Aircraft to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}", Name = "PutAircraftRoute")]
        public async Task<IActionResult> PutAircraft(int logbookId, int aircraftId, [FromBody] Aircraft aircraft)
        {
            // Validate request data
            if (aircraft == null)
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
                        _logger.LogInformation("[PutAircraft] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Aircraft existingAircraft = await _apiDbContext.Aircraft
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AircraftId == aircraftId && a.LogbookId == logbookId);

            // Return not found result
            if (existingAircraft == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingAircraft.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            aircraft.AircraftId = existingAircraft.AircraftId;
            aircraft.LogbookId = existingAircraft.LogbookId;
            aircraft.UserId = existingAircraft.UserId;
            aircraft.CreatedOn = existingAircraft.CreatedOn;
            aircraft.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(aircraft).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutAircraft]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Aircraft resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="aircraftId">Unique aircraft identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}", Name = "DeleteAircraftRoute")]
        public async Task<IActionResult> DeleteAircraft(int logbookId, int aircraftId)
        {
            // Check to see if the resource exists
            Aircraft existingAircraft = await _apiDbContext.Aircraft
                .FirstOrDefaultAsync(a => a.AircraftId == aircraftId && a.LogbookId == logbookId);

            // Return not found result
            if (existingAircraft == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingAircraft.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Aircraft.Remove(existingAircraft);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteAircraft]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            AircraftDTO dto = AircraftDTO.ToDto(existingAircraft);

            // Return result
            return Ok(dto);
        }
    }
}
