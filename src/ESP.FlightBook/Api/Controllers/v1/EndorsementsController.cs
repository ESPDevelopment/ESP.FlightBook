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
    public class EndorsementsController : LogbookApiController
    {
        protected readonly ILogger<EndorsementsController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public EndorsementsController(ApplicationDbContext apiDbContext, ILogger<EndorsementsController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Endorsement resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - The associated logbook was not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/endorsements<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/endorsements", Name = "GetAllEndorsementsRoute")]
        public async Task<IActionResult> GetAllEndorsements(int logbookId)
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

            // Find specified resources
            List<Endorsement> endorsementList = await _apiDbContext.Endorsements
                .AsNoTracking()
                .Where(e => e.LogbookId == logbookId)
                .ToListAsync();

            // Convert to DTO
            List<EndorsementDTO> dtoList = new List<EndorsementDTO>();
            foreach(Endorsement endorsement in endorsementList)
            {
                dtoList.Add(EndorsementDTO.ToDto(endorsement));
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Endorsement resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="endorsementId">Unique endorsement identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}", Name = "GetEndorsementByIdRoute")]
        public async Task<IActionResult> GetEndorsement(int logbookId, int endorsementId)
        {
            // Find specified resources
            Endorsement endorsement = await _apiDbContext.Endorsements
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EndorsementId == endorsementId && e.LogbookId == logbookId);

            // Return not found result
            if (endorsement == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = endorsement.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            EndorsementDTO dto = EndorsementDTO.ToDto(endorsement);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Endorsement resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="endorsement">The endorsement to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/endorsements</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/endorsements", Name = "PostEndorsementRoute")]
        public async Task<IActionResult> PostEndorsement(int logbookId, [FromBody] Endorsement endorsement)
        {
            // Validate request data
            if (endorsement == null)
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
                        _logger.LogInformation("[PostEndorsement] {0}", message.ErrorMessage);
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
            endorsement.LogbookId = logbookId;
            endorsement.UserId = GetUserIdClaim();
            endorsement.CreatedOn = DateTime.UtcNow;
            endorsement.ChangedOn = endorsement.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Endorsements.Add(endorsement);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostEndorsement]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            EndorsementDTO dto = EndorsementDTO.ToDto(endorsement);

            // Return success
            return CreatedAtRoute("GetEndorsementByIdRoute",
                new
                {
                    logbookId = dto.LogbookId,
                    endorsementId = dto.EndorsementId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Endorsement resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="endorsementId">Unique endorsement identifier</param>
        /// <param name="endorsement">Endorsement to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}", Name = "PutEndorsementRoute")]
        public async Task<IActionResult> PutEndorsement(int logbookId, int endorsementId, [FromBody] Endorsement endorsement)
        {
            // Validate request data
            if (endorsement == null)
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
                        _logger.LogInformation("[PutEndorsement] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Endorsement existingEndorsement = await _apiDbContext.Endorsements
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EndorsementId == endorsementId && e.LogbookId == logbookId);

            // Return not found result
            if (existingEndorsement == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingEndorsement.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            endorsement.EndorsementId = existingEndorsement.EndorsementId;
            endorsement.LogbookId = existingEndorsement.LogbookId;
            endorsement.UserId = existingEndorsement.UserId;
            endorsement.CreatedOn = existingEndorsement.CreatedOn;
            endorsement.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(endorsement).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutEndorsement]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Endorsement resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="endorsementId">Unique endorsement identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}", Name = "DeleteEndorsementRoute")]
        public async Task<IActionResult> DeleteEndorsement(int logbookId, int endorsementId)
        {
            // Check to see if the resource exists
            Endorsement existingEndorsement = await _apiDbContext.Endorsements
                .FirstOrDefaultAsync(e => e.EndorsementId == endorsementId && e.LogbookId == logbookId);

            // Return not found result
            if (existingEndorsement == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingEndorsement.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Endorsements.Remove(existingEndorsement);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteEndorsement]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            EndorsementDTO dto = EndorsementDTO.ToDto(existingEndorsement);

            // Return result
            return Ok(dto);
        }
    }
}
