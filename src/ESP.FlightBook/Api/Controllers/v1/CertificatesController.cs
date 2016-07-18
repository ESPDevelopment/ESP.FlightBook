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
    public class CertificatesController : LogbookApiController
    {
        protected readonly ILogger<CertificatesController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public CertificatesController(ApplicationDbContext apiDbContext, ILogger<CertificatesController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns specified Certificate resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - The associated logbook is not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/certificates<remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/certificates", Name = "GetAllCertificatesRoute")]
        public async Task<IActionResult> GetAllCertificates(int logbookId)
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
            List<Certificate> certificateList = await _apiDbContext.Certificates
                .AsNoTracking()
                .Include(c => c.Ratings)
                .Where(c => c.LogbookId == logbookId)
                .ToListAsync();

            // Convert to DTO
            List<CertificateDTO> dtoList = new List<CertificateDTO>();
            foreach (Certificate certificate in certificateList)
            {
                CertificateDTO dto = CertificateDTO.ToDto(certificate);
                foreach(Rating rating in certificate.Ratings)
                {
                    dto.Ratings.Add(RatingDTO.ToDto(rating));
                }
                dtoList.Add(dto);
            }

            // Return the result
            return Ok(dtoList);
        }

        /// <summary>
        /// Returns specified Certificate resources
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <returns>
        /// (200) OK - Returns the requested reource
        /// (403) Forbidden - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}</remarks>
        [HttpGet("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}", Name = "GetCertificateByIdRoute")]
        public async Task<IActionResult> GetCertificate(int logbookId, int certificateId)
        {
            // Find specified resources
            Certificate certificate = await _apiDbContext.Certificates
                .AsNoTracking()
                .Include(c => c.Ratings)
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId && c.LogbookId == logbookId);

            // Return not found result
            if (certificate == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = certificate.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Convert to DTO
            CertificateDTO dto = CertificateDTO.ToDto(certificate);
            foreach(Rating rating in certificate.Ratings)
            {
                dto.Ratings.Add(RatingDTO.ToDto(rating));
            }

            // Return the result
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Certificate resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificate">The certificate to be created</param>
        /// <returns>
        /// (201) Created - If the resource was successfully created
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not Found - If the associated logbook is not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>POST api/v1/logbooks/{logbookId:int}/certificates</remarks>
        [HttpPost("api/v1/logbooks/{logbookId:int}/certificates", Name = "PostCertificateRoute")]
        public async Task<IActionResult> PostCertificate(int logbookId, [FromBody] Certificate certificate)
        {
            // Validate request data
            if (certificate == null)
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
                        _logger.LogInformation("[PostCertificate] {0}", message.ErrorMessage);
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
            certificate.LogbookId = logbookId;
            certificate.UserId = GetUserIdClaim();
            certificate.CreatedOn = DateTime.UtcNow;
            certificate.ChangedOn = certificate.CreatedOn;
            if (certificate.Ratings != null)
            {
                foreach (Rating rating in certificate.Ratings)
                {
                    rating.RatingId = 0;
                    rating.UserId = certificate.UserId;
                }
            }

            // Attempt to add new resource
            _apiDbContext.Certificates.Add(certificate);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PostCertificate]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            CertificateDTO dto = CertificateDTO.ToDto(certificate);
            if (certificate.Ratings != null)
            {
                foreach (Rating rating in certificate.Ratings)
                {
                    dto.Ratings.Add(RatingDTO.ToDto(rating));
                }
            }

            // Return success
            return CreatedAtRoute("GetCertificateByIdRoute",
                new
                {
                    logbookId = dto.LogbookId,
                    certificateId = dto.CertificateId
                }, dto);
        }

        /// <summary>
        /// Updates an existing Certificate resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <param name="certificate">Certificate to be udpated</param>
        /// <returns>
        /// (204) No Content - If the resource was updated successfully
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}</remarks>
        [HttpPut("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}", Name = "PutCertificateRoute")]
        public async Task<IActionResult> PutCertificate(int logbookId, int certificateId, [FromBody] Certificate certificate)
        {
            // Validate request data
            if (certificate == null)
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
                        _logger.LogInformation("[PutCertificate] {0}", message.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            // Find specified resource
            Certificate existingCertificate = await _apiDbContext.Certificates
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId && c.LogbookId == logbookId);

            // Return not found result
            if (existingCertificate == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingCertificate.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Set non-editable attributes
            certificate.CertificateId = existingCertificate.CertificateId;
            certificate.LogbookId = existingCertificate.LogbookId;
            certificate.UserId = existingCertificate.UserId;
            certificate.CreatedOn = existingCertificate.CreatedOn;
            certificate.ChangedOn = DateTime.UtcNow;
            if (certificate.Ratings != null)
            {
                foreach (Rating rating in certificate.Ratings)
                {
                    rating.RatingId = 0;
                    rating.UserId = certificate.UserId;
                }
            }

            // Attempt to update resource
            _apiDbContext.Entry(certificate).State = EntityState.Modified;
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[PutCertificate]", e);
                return new StatusCodeResult(409);
            }

            // Return the result
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes an existing Certificate resource for the specified Logbook
        /// </summary>
        /// <param name="logbookId">Unique logbook identifier</param>
        /// <param name="certificateId">Unique certificate identifier</param>
        /// <returns>
        /// (200) OK - If the resource was successfully deleted
        /// (400) Bad Request - If the specified resource data was not valid
        /// (403) Forbidden - If the request is not authorized
        /// (404) Not found - If the resource was not found
        /// (409) Conflict - If the resource could not be created
        /// <remarks>DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}</remarks>
        [HttpDelete("api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}", Name = "DeleteCertificateRoute")]
        public async Task<IActionResult> DeleteCertificate(int logbookId, int certificateId)
        {
            // Check to see if the resource exists
            Certificate existingCertificate = await _apiDbContext.Certificates
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId && c.LogbookId == logbookId);

            // Return not found result
            if (existingCertificate == null)
            {
                return NotFound();
            }

            // Authorize the request
            bool isAuthorized = existingCertificate.UserId.Equals(GetUserIdClaim());

            // Return forbidden result
            if (isAuthorized == false)
            {
                return new StatusCodeResult(403);
            }

            // Attempt to delete the resource
            _apiDbContext.Certificates.Remove(existingCertificate);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("[DeleteCertificate]", e);
                return new StatusCodeResult(409);
            }

            // Convert to DTO
            CertificateDTO dto = CertificateDTO.ToDto(existingCertificate);

            // Return result
            return Ok(dto);
        }
    }
}
