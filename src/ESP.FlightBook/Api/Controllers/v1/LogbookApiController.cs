using ESP.FlightBook.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace ESP.FlightBook.Api.Controllers.v1
{
    [Authorize("Bearer")]
    public class LogbookApiController : Controller
    {
        protected readonly ApplicationDbContext _apiDbContext;

        /// <summary>
        /// Constructs a controller with the given application database context
        /// </summary>
        /// <param name="context"></param>
        public LogbookApiController(ApplicationDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        /// <summary>
        /// Releases appropriate resources when the controller is disposed of.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _apiDbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the value of the NameIdentifier from the identity claim. This value
        /// represents the unique identifier of the registered user.
        /// </summary>
        /// <returns>The user id if a valid claim is present, otherwise null</returns>
        protected string GetUserIdClaim()
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return null;
            }

            // Validate the issuer
            if (claim.Issuer != "http://espdevelopment.com")
            {
                return null;
            }

            // Return the user id
            return claim.Value;
        }

        /// <summary>
        /// Returns the number of "pages" necessary given a specific row count and
        /// a requested size (number of rows) per page
        /// </summary>
        /// <param name="rowCount">Total number of rows</param>
        /// <param name="requestedPageSize">Requested number of rows per page</param>
        /// <returns>Total number of pages required.</returns>
        protected int TotalPageCount(int rowCount, int requestedPageSize)
        {
            int returnValue;
            if (rowCount > 0)
            {
                if (rowCount <= requestedPageSize)
                {
                    return 1;
                }
                else
                {
                    if ((rowCount % requestedPageSize) == 0)
                    {
                        returnValue = rowCount / requestedPageSize;
                    }
                    else
                    {
                        decimal decimalRowCount = Convert.ToDecimal(rowCount);
                        decimal decimalRequestedPageSize = Convert.ToDecimal(requestedPageSize);
                        decimal resultRounded = Math.Round(decimalRowCount / decimalRequestedPageSize);
                        decimal resultNotRounded = (decimalRowCount / decimalRequestedPageSize);
                        if (resultRounded < resultNotRounded)
                        {
                            returnValue = Convert.ToInt32(resultRounded + 1);
                        }
                        else
                        {
                            returnValue = Convert.ToInt32(resultRounded);
                        }
                    }
                }
            }
            else
            {
                returnValue = 0;
            }
            return returnValue;
        }

        /// <summary>
        /// Returns the best page number given the total number of pages and the requested
        /// page.  Essentially, ensures that if the request page is larger than the number of
        /// available pages then the last page will be selected.
        /// </summary>
        /// <param name="totalCountOfPages"></param>
        /// <param name="requestedPage"></param>
        /// <returns></returns>
        protected int GetBestPage(int totalCountOfPages, int requestedPage)
        {
            if (requestedPage > 0 && totalCountOfPages > 0)
            {
                requestedPage = totalCountOfPages < requestedPage ? totalCountOfPages : requestedPage;
                requestedPage = totalCountOfPages != requestedPage ? requestedPage : totalCountOfPages - 1;
            }
            else
            {
                requestedPage = 0;
            }
            return requestedPage;
        }
    }
}
