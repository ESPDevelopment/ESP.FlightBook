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
using Xunit;

namespace ESP.FlightBook.Tests.Api.Controllers
{
    [Collection("Controller Tests")]
    public class PilotsControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<PilotsController> _logger;
        private PilotsController _controller;

        /// <summary>
        /// Constructs a new PilotsControllerTest object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and PilotsController
        /// objects for every test.
        /// </summary>
        public PilotsControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<PilotsController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new PilotsController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/pilots
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetPilotByUserIdTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetPilot_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            // Act
            var result = _controller.GetPilot(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetPilot_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetPilot(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetPilot_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetPilot(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetPilot_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetPilot(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.Equal((result.Value as PilotDTO).AddressLine1, pilot.AddressLine1);
            Assert.Equal((result.Value as PilotDTO).AddressLine2, pilot.AddressLine2);
            Assert.Equal((result.Value as PilotDTO).CellPhoneNumber, pilot.CellPhoneNumber);
            Assert.Equal((result.Value as PilotDTO).ChangedOn, pilot.ChangedOn);
            Assert.Equal((result.Value as PilotDTO).City, pilot.City);
            Assert.Equal((result.Value as PilotDTO).Country, pilot.Country);
            Assert.Equal((result.Value as PilotDTO).CreatedOn, pilot.CreatedOn);
            Assert.Equal((result.Value as PilotDTO).EmailAddress, pilot.EmailAddress);
            Assert.Equal((result.Value as PilotDTO).FirstName, pilot.FirstName);
            Assert.Equal((result.Value as PilotDTO).HomePhoneNumber, pilot.HomePhoneNumber);
            Assert.Equal((result.Value as PilotDTO).LastName, pilot.LastName);
            Assert.Equal((result.Value as PilotDTO).Logbook, null);
            Assert.Equal((result.Value as PilotDTO).LogbookId, pilot.LogbookId);
            Assert.Equal((result.Value as PilotDTO).PilotId, pilot.PilotId);
            Assert.Equal((result.Value as PilotDTO).PostalCode, pilot.PostalCode);
            Assert.Equal((result.Value as PilotDTO).StateOrProvince, pilot.StateOrProvince);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetPilotByIdTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetPilotById_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            // Act
            var result = _controller.GetPilot(pilot.LogbookId, pilot.PilotId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetPilotById_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetPilot(pilot.LogbookId, pilot.PilotId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetPilotById_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetPilot(pilot.LogbookId, pilot.PilotId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetPilotById_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetPilot(pilot.LogbookId, pilot.PilotId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.Equal((result.Value as PilotDTO).AddressLine1, pilot.AddressLine1);
            Assert.Equal((result.Value as PilotDTO).AddressLine2, pilot.AddressLine2);
            Assert.Equal((result.Value as PilotDTO).CellPhoneNumber, pilot.CellPhoneNumber);
            Assert.Equal((result.Value as PilotDTO).ChangedOn, pilot.ChangedOn);
            Assert.Equal((result.Value as PilotDTO).City, pilot.City);
            Assert.Equal((result.Value as PilotDTO).Country, pilot.Country);
            Assert.Equal((result.Value as PilotDTO).CreatedOn, pilot.CreatedOn);
            Assert.Equal((result.Value as PilotDTO).EmailAddress, pilot.EmailAddress);
            Assert.Equal((result.Value as PilotDTO).FirstName, pilot.FirstName);
            Assert.Equal((result.Value as PilotDTO).HomePhoneNumber, pilot.HomePhoneNumber);
            Assert.Equal((result.Value as PilotDTO).LastName, pilot.LastName);
            Assert.Equal((result.Value as PilotDTO).Logbook, null);
            Assert.Equal((result.Value as PilotDTO).LogbookId, pilot.LogbookId);
            Assert.Equal((result.Value as PilotDTO).PilotId, pilot.PilotId);
            Assert.Equal((result.Value as PilotDTO).PostalCode, pilot.PostalCode);
            Assert.Equal((result.Value as PilotDTO).StateOrProvince, pilot.StateOrProvince);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/pilots
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostPilotTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostPilot_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            // Act
            var result = _controller.PostPilot(logbook.LogbookId, pilot).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostPilot_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostPilot(logbook.LogbookId, pilot).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostPilot_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostPilot(logbook.LogbookId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostPilot_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostPilot(logbook.LogbookId, pilot).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostPilot_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostPilot(logbook.LogbookId, pilot).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetPilotByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], logbook.LogbookId);
            Assert.Equal(result.RouteValues["pilotId"], pilot.PilotId);
            Assert.Equal((result.Value as PilotDTO).AddressLine1, pilot.AddressLine1);
            Assert.Equal((result.Value as PilotDTO).AddressLine2, pilot.AddressLine2);
            Assert.Equal((result.Value as PilotDTO).CellPhoneNumber, pilot.CellPhoneNumber);
            Assert.Equal((result.Value as PilotDTO).ChangedOn, pilot.ChangedOn);
            Assert.Equal((result.Value as PilotDTO).City, pilot.City);
            Assert.Equal((result.Value as PilotDTO).Country, pilot.Country);
            Assert.Equal((result.Value as PilotDTO).CreatedOn, pilot.CreatedOn);
            Assert.Equal((result.Value as PilotDTO).EmailAddress, pilot.EmailAddress);
            Assert.Equal((result.Value as PilotDTO).FirstName, pilot.FirstName);
            Assert.Equal((result.Value as PilotDTO).HomePhoneNumber, pilot.HomePhoneNumber);
            Assert.Equal((result.Value as PilotDTO).LastName, pilot.LastName);
            Assert.Equal((result.Value as PilotDTO).Logbook, null);
            Assert.Equal((result.Value as PilotDTO).LogbookId, pilot.LogbookId);
            Assert.Equal((result.Value as PilotDTO).PilotId, pilot.PilotId);
            Assert.Equal((result.Value as PilotDTO).PostalCode, pilot.PostalCode);
            Assert.Equal((result.Value as PilotDTO).StateOrProvince, pilot.StateOrProvince);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutPilotTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutPilot_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            // Act
            var result = _controller.PutPilot(pilot.LogbookId, pilot.PilotId, pilot).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutPilot_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutPilot(pilot.LogbookId, pilot.PilotId, pilot).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutPilot_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutPilot(pilot.LogbookId, pilot.PilotId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutPilot_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutPilot(pilot.LogbookId, pilot.PilotId, pilot).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutPilot_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutPilot(pilot.LogbookId, pilot.PilotId, pilot).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/pilots
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutPilot_ShouldReturnNoContentOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutPilot(pilot.LogbookId, pilot.PilotId, pilot).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeletePilotTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeletePilot_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            // Act
            var result = _controller.DeletePilot(pilot.LogbookId, pilot.PilotId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeletePilot_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeletePilot(pilot.LogbookId, pilot.PilotId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeletePilot_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeletePilot(pilot.LogbookId, pilot.PilotId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/pilots/{pilotId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeletePilot_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Pilot pilot = MockDataFactory.CreateMockPilot(userId, 1, logbook.LogbookId);
            _apiDbContext.Pilots.Add(pilot);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(pilot).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeletePilot(pilot.LogbookId, pilot.PilotId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.Equal((result.Value as PilotDTO).AddressLine1, pilot.AddressLine1);
            Assert.Equal((result.Value as PilotDTO).AddressLine2, pilot.AddressLine2);
            Assert.Equal((result.Value as PilotDTO).CellPhoneNumber, pilot.CellPhoneNumber);
            Assert.Equal((result.Value as PilotDTO).ChangedOn, pilot.ChangedOn);
            Assert.Equal((result.Value as PilotDTO).City, pilot.City);
            Assert.Equal((result.Value as PilotDTO).Country, pilot.Country);
            Assert.Equal((result.Value as PilotDTO).CreatedOn, pilot.CreatedOn);
            Assert.Equal((result.Value as PilotDTO).EmailAddress, pilot.EmailAddress);
            Assert.Equal((result.Value as PilotDTO).FirstName, pilot.FirstName);
            Assert.Equal((result.Value as PilotDTO).HomePhoneNumber, pilot.HomePhoneNumber);
            Assert.Equal((result.Value as PilotDTO).LastName, pilot.LastName);
            Assert.Equal((result.Value as PilotDTO).Logbook, null);
            Assert.Equal((result.Value as PilotDTO).LogbookId, pilot.LogbookId);
            Assert.Equal((result.Value as PilotDTO).PilotId, pilot.PilotId);
            Assert.Equal((result.Value as PilotDTO).PostalCode, pilot.PostalCode);
            Assert.Equal((result.Value as PilotDTO).StateOrProvince, pilot.StateOrProvince);
        }

        #endregion
    }
}
