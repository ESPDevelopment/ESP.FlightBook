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
    public class RatingsController : LogbookApiController
    {
        protected readonly ILogger<RatingsController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public RatingsController(ApplicationDbContext apiDbContext, ILogger<RatingsController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Ratings resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings", Name = "GetAllRatingsRoute")]
        public async Task<IActionResult> GetAllRatings(int logbookId, int certificateId)
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
            List<Rating> ratingList = await _apiDbContext.Ratings
                .AsNoTracking()
                .Where(r => r.CertificateId == certificateId && r.Certificate.LogbookId == logbookId)
                .ToListAsync();

            // Convert to DTO
            List<RatingDTO> dtoList = new List<RatingDTO>();
            foreach (Rating rating in ratingList)
            {
                dtoList.Add(RatingDTO.ToDto(rating));
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Rating resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="ratingId">Unique rating identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Unauthorized - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}", Name = "GetRatingByIdRoute")]
        public async Task<IActionResult> GetRating(int logbookId, int certificateId, int ratingId)
        {
            // Find specified resources
            Rating rating = await _apiDbContext.Ratings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RatingId == ratingId && r.CertificateId == certificateId && r.Certificate.LogbookId == logbookId);

            // Return not found result
            if (rating == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = rating.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            RatingDTO dto = RatingDTO.ToDto(rating);

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Rating resource for the specified Certificate
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="rating">The rating to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings", Name = "PostRatingRoute")]
        public async Task<IActionResult> PostRating(int logbookId, int certificateId, [FromBody] Rating rating)
        {
            // Validate request data
            if (rating == null)
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
                        _logger.LogInformation("[PostRating] {0}", message.ErrorMessage);
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
            rating.CertificateId = certificateId;
            rating.UserId = GetUserIdClaim();
            rating.CreatedOn = DateTime.UtcNow;
            rating.ChangedOn = rating.CreatedOn;

            // Attempt to add new resource
            _apiDbContext.Ratings.Add(rating);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostRating]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            RatingDTO dto = RatingDTO.ToDto(rating);

            // Return success
            return CreatedAtRoute("GetRatingByIdRoute",
                new
                {
                    logbookId = logbookId,
                    certificateId = dto.CertificateId,
                    ratingId = dto.RatingId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Rating resource for the specified Certificate
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="ratingId">Unique rating identifier</param>
        /// <param name="rating">Rating to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}", Name = "PutRatingRoute")]
        public async Task<IActionResult> PutRating(int logbookId, int certificateId, int ratingId, [FromBody] Rating rating)
        {
            // Validate request data
            if (rating == null)
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
                        _logger.LogInformation("[PutRating] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Rating existingRating = await _apiDbContext.Ratings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RatingId == ratingId && r.CertificateId == certificateId && r.Certificate.LogbookId == logbookId);

            // Return not found result
            if (existingRating == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingRating.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            rating.RatingId = existingRating.RatingId;
            rating.CertificateId = existingRating.CertificateId;
            rating.UserId = existingRating.UserId;
            rating.CreatedOn = existingRating.CreatedOn;
            rating.ChangedOn = DateTime.UtcNow;

            // Attempt to update resource
            _apiDbContext.Entry(rating).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutRating]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Rating resource for the specified Certificate
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="ratingId">Unique rating identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}", Name = "DeleteRatingRoute")]
        public async Task<IActionResult> DeleteRating(int logbookId, int certificateId, int ratingId)
        {
            // Check to see if the resource exists
            Rating existingRating = await _apiDbContext.Ratings
                .FirstOrDefaultAsync(r => r.RatingId == ratingId && r.CertificateId == certificateId && r.Certificate.LogbookId == logbookId);

            // Return not found result
            if (existingRating == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingRating.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Ratings.Remove(existingRating);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteRating]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            RatingDTO dto = RatingDTO.ToDto(existingRating);

            // Return result
            return Ok(dto);
        }
    }
}
