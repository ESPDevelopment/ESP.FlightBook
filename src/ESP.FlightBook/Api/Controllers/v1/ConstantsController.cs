using ESP.FlightBook.Api.ViewModels;
using ESP.FlightBook.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ESP.FlightBook.Api.Controllers.v1
{
    public class ConstantsController : LogbookApiController
    {
        protected readonly ILogger<ConstantsController> _logger;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public ConstantsController(ApplicationDbContext apiDbContext, ILogger<ConstantsController> logger)
            : base(apiDbContext)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns the constants resource for the application
        /// </summary>
        /// <returns>
        /// (200) OK - Returns the requested resources
        /// (401) Unauthorized - The request is not authorized
        /// (404) Not Found - Resource not found
        /// </returns>
        /// <remarks>GET: api/Constants</remarks>
        [AllowAnonymous]
        [HttpGet("/api/v1/constants", Name = "GetConstantsRoute")]
        public async Task<IActionResult> GetConstants()
        {
            // Create the view model
            ConstantsViewModel viewModel = new ConstantsViewModel();

            // Retrieve approach types
            viewModel.ApproachTypes = await _apiDbContext.ApproachTypes
                .OrderBy(a => a.SortOrder)
                .ToListAsync();

            // Retrieve aircraft categories and classes
            viewModel.CategoriesAndClasses = await _apiDbContext.CategoriesAndClasses
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Class)
                .ToListAsync();

            // Retrieve certificate types
            viewModel.CertificateTypes = await _apiDbContext.CertificateTypes
                .OrderBy(ct => ct.SortOrder)
                .ToListAsync();

            // Retrieve endorsement types
            viewModel.EndorsementTypes = await _apiDbContext.EndorsementTypes
                .OrderBy(et => et.SortOrder)
                .ToListAsync();

            // Retrieve engine types
            viewModel.EngineTypes = await _apiDbContext.EngineTypes
                .OrderBy(et => et.SortOrder)
                .ToListAsync();

            // Retrieve gear types
            viewModel.GearTypes = await _apiDbContext.GearTypes
                .OrderBy(gt => gt.SortOrder)
                .ToListAsync();

            // Retrieve rating types
            viewModel.RatingTypes = await _apiDbContext.RatingTypes
                .OrderBy(rt => rt.SortOrder)
                .ToListAsync();

            // Retrieve currency types
            viewModel.CurrencyTypes = await _apiDbContext.CurrencyTypes
                .OrderBy(ct => ct.SortOrder)
                .ToListAsync();

            // Return the view model
            return Ok(viewModel);
        }
    }
}
