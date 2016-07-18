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
    public class LogbookControllers : LogbookApiController
    {
        protected readonly ILogger<LogbookControllers> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public LogbookControllers(ApplicationDbContext apiDbContext, ILogger<LogbookControllers> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns all logbooks for the authenticated user
        /// </summary>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (401) Unauthorized - The request is not authorized
        /// </returns>
        /// <remarks>GET: api/v1/logbook</remarks>
        [HttpGet("api/v1/logbooks", Name = "GetAllLogbooksRoute")]
        public async Task<IActionResult> GetAllLogbooks()
        {
            string userId = GetUserIdClaim();

            // Validate user id claim
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Find specified resources
            List<Logbook> logbookList = await _apiDbContext.Logbooks
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .ToListAsync();

            // Convert to DTO
            List<LogbookDTO> dto = new List<LogbookDTO>();
            foreach (Logbook logbook in logbookList)
            {
                dto.Add(LogbookDTO.ToDto(logbook));
            }

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Returns specified logbook resource
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET: api/v1/logbook/{logbookId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}", Name = "GetLogbookByIdRoute")]
        public async Task<IActionResult> GetLogbook(int logbookId)
        {
            // Find specified resources
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

            // Convert to DTO
            LogbookDTO dto = LogbookDTO.ToDto(logbook);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Logbook resource for the specified user
        /// </summary>
        /// <param name="logbook">The logbook to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST: api/v1/logbooks</remarks>
        [HttpPost("api/v1/logbooks", Name = "PostLogbookRoute")]
        public async Task<IActionResult> PostLogbook([FromBody] Logbook logbook)
        {
            // Validate request data
            if (logbook == null)
            {
                return BadRequest();
            }

            // Authorize the request
            bool isAuthorized = !string.IsNullOrEmpty(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Ignore user id
            ModelState.Remove("UserId");
            ModelState.Remove("");

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var message in error.Errors)
                    {
                        _logger.LogInformation("[PostLogbook] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Set non-editable attributes
            logbook.UserId = GetUserIdClaim();
            logbook.CreatedOn = DateTime.UtcNow;
            logbook.ChangedOn = logbook.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Logbooks.Add(logbook);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostLogbook]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            LogbookDTO dto = LogbookDTO.ToDto(logbook);

            // Return success
            return CreatedAtRoute("GetLogbookByIdRoute",
                new
                {
                    logbookId = dto.LogbookId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Logbook resource for the specified pilot
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="logbook">Logbook to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT: api/v1/logbooks/{logbookId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}", Name = "PutLogbookRoute")]
        public async Task<IActionResult> PutLogbook(int logbookId, [FromBody] Logbook logbook)
        {
            // Find specified resource
            Logbook existingLogbook = await _apiDbContext.Logbooks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (existingLogbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingLogbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Validate request data
            if (logbook == null)
            {
                return BadRequest();
            }

            // Ignore user id
            ModelState.Remove("UserId");
            ModelState.Remove("");

            // Validate the model
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values)
                {
                    foreach (var message in error.Errors)
                    {
                        _logger.LogInformation("[PutLogbook] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Set non-editable attributes
            logbook.LogbookId = existingLogbook.LogbookId;
            logbook.UserId = existingLogbook.UserId;
            logbook.CreatedOn = existingLogbook.CreatedOn;
            logbook.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(logbook).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutLogbook]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Logbook resource for the specified user
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Unauthorized - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>api/v1/logbooks/{logbookId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}", Name = "DeleteLogbookRoute")]
        public async Task<IActionResult> DeleteLogbook(int logbookId)
        {
            // Check to see if the resource exists
            Logbook existingLogbook = await _apiDbContext.Logbooks
                .FirstOrDefaultAsync(l => l.LogbookId == logbookId);

            // Return not found result
            if (existingLogbook == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingLogbook.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Logbooks.Remove(existingLogbook);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteLogbook]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            LogbookDTO dto = LogbookDTO.ToDto(existingLogbook);

            // Return result
            return Ok(dto);
        }
    }
}
