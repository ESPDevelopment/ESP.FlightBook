using ESP.FlightBook.Api.Controllers.v1;
using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Api.ViewModels;
using ESP.FlightBook.Data;
using ESP.FlightBook.Tests.Api.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESP.FlightBook.Tests.Api.Controllers
{
    [Collection("Controller Tests")]
    public class LogbookControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<LogbookControllers> _logger;
        private LogbookControllers _controller;

        /// <summary>
        /// Constructs a new LogbooksControllerUnitTests object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and LogbooksController
        /// objects for every test.
        /// </summary>
        public LogbookControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<LogbookControllers>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new LogbookControllers(_apiDbContext, _logger);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        /// <summary>
        /// Disposes resources between tests.  The XUnit framework calls the class constructor
        /// for each test which allows us to dispose of the in-memory database between tests
        /// </summary>
        public void Dispose()
        {
            // Dispose the in-memory database
            _apiDbContext.Dispose();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllLogbookTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks
        /// returns a 401 Unauthorized response when the identity is missing
        /// </summary>
        [Fact]
        public void GetAllLogbooks_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            // Act
            var result = _controller.GetAllLogbooks().Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(UnauthorizedResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetAllLogbooks_ShouldReturnEmptyListOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllLogbooks().Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<LogbookDTO>));
            Assert.Equal((result.Value as List<LogbookDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllLogbooks_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook1 = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook1).State = EntityState.Detached;
            Logbook logbook2 = MockDataFactory.CreateMockLogbook(userId, 2);
            _apiDbContext.Logbooks.Add(logbook2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook2).State = EntityState.Detached;
            Logbook logbook3 = MockDataFactory.CreateMockLogbook(userId, 3);
            _apiDbContext.Logbooks.Add(logbook3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllLogbooks().Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<LogbookDTO>));
            Assert.Equal((result.Value as List<LogbookDTO>).Count, 3);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetLogbookTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetLogbook_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            // Act
            var result = _controller.GetLogbook(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetLogbook_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetLogbook(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetLogbook_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetLogbook(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetLogbook_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetLogbook(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as LogbookDTO).Aircraft);
            Assert.NotNull((result.Value as LogbookDTO).Certificates);
            Assert.Equal((result.Value as LogbookDTO).ChangedOn, logbook.ChangedOn);
            Assert.Equal((result.Value as LogbookDTO).CreatedOn, logbook.CreatedOn);
            Assert.NotNull((result.Value as LogbookDTO).Endorsements);
            Assert.NotNull((result.Value as LogbookDTO).Flights);
            Assert.Equal((result.Value as LogbookDTO).LogbookId, logbook.LogbookId);
            Assert.Null((result.Value as LogbookDTO).Pilot);
            Assert.Equal((result.Value as LogbookDTO).Remarks, logbook.Remarks);
            Assert.Equal((result.Value as LogbookDTO).Title, logbook.Title);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostLogbookTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostLogbook_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            // Act
            var result = _controller.PostLogbook(logbook).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostLogbook_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostLogbook(null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostLogbook_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostLogbook(logbook).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostLogbook_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostLogbook(logbook).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetLogbookByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], logbook.LogbookId);
            Assert.NotNull((result.Value as LogbookDTO).Aircraft);
            Assert.NotNull((result.Value as LogbookDTO).Certificates);
            Assert.Equal((result.Value as LogbookDTO).ChangedOn, logbook.ChangedOn);
            Assert.Equal((result.Value as LogbookDTO).CreatedOn, logbook.CreatedOn);
            Assert.NotNull((result.Value as LogbookDTO).Endorsements);
            Assert.NotNull((result.Value as LogbookDTO).Flights);
            Assert.Equal((result.Value as LogbookDTO).LogbookId, logbook.LogbookId);
            Assert.Null((result.Value as LogbookDTO).Pilot);
            Assert.Equal((result.Value as LogbookDTO).Remarks, logbook.Remarks);
            Assert.Equal((result.Value as LogbookDTO).Title, logbook.Title);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutLogbookTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutLogbook_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            // Act
            var result = _controller.PutLogbook(logbook.LogbookId, logbook).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutLogbook_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutLogbook(logbook.LogbookId, logbook).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutLogbok_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutLogbook(logbook.LogbookId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutLogbook_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutLogbook(logbook.LogbookId, logbook).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutLogbook_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutLogbook(logbook.LogbookId, logbook).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutLogbook_ShouldReturnNoContentOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutLogbook(logbook.LogbookId, logbook).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteLogbookTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteLogbook_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            // Act
            var result = _controller.DeleteLogbook(logbook.LogbookId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteLogbook_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteLogbook(logbook.LogbookId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteLogbook_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteLogbook(logbook.LogbookId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteLogbook_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteLogbook(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as LogbookDTO).Aircraft);
            Assert.NotNull((result.Value as LogbookDTO).Certificates);
            Assert.Equal((result.Value as LogbookDTO).ChangedOn, logbook.ChangedOn);
            Assert.Equal((result.Value as LogbookDTO).CreatedOn, logbook.CreatedOn);
            Assert.NotNull((result.Value as LogbookDTO).Endorsements);
            Assert.NotNull((result.Value as LogbookDTO).Flights);
            Assert.Equal((result.Value as LogbookDTO).LogbookId, logbook.LogbookId);
            Assert.Null((result.Value as LogbookDTO).Pilot);
            Assert.Equal((result.Value as LogbookDTO).Remarks, logbook.Remarks);
            Assert.Equal((result.Value as LogbookDTO).Title, logbook.Title);
        }

        #endregion
    }
}
