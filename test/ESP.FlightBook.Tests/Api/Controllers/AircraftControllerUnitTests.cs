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
    public class AircraftControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<AircraftController> _logger;
        private AircraftController _controller;

        /// <summary>
        /// Constructs a new PilotsControllerTest object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and PilotsController
        /// objects for every test.
        /// </summary>
        public AircraftControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<AircraftController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new AircraftController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/aircraft
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllAircraftTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAllAircraft_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            // Act
            var result = _controller.GetAllAircraft(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAllAircraft_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAllAircraft(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 200 Ok response and an empty list when the requested resources are not found.
        /// </summary>
        [Fact]
        public void GetAllAircraft_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllAircraft(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(List<AircraftDTO>), result.Value);
            Assert.Equal((result.Value as List<AircraftDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllAircraft_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft1 = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft1).State = EntityState.Detached;
            Aircraft aircraft2 = MockDataFactory.CreateMockAircraft(userId, 2, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft2).State = EntityState.Detached;
            Aircraft aircraft3 = MockDataFactory.CreateMockAircraft(userId, 3, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllAircraft(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<AircraftDTO>));
            Assert.Equal((result.Value as List<AircraftDTO>).Count, 3);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAircraftTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAircraft_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            // Act
            var result = _controller.GetAircraft(aircraft.LogbookId, aircraft.AircraftId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAircraft_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAircraft(aircraft.LogbookId, aircraft.AircraftId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetAircraft_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAircraft(aircraft.LogbookId, aircraft.AircraftId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAircraft_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAircraft(aircraft.LogbookId, aircraft.AircraftId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(AircraftDTO), result.Value);
            Assert.Equal((result.Value as AircraftDTO).AircraftCategory, aircraft.AircraftCategory);
            Assert.Equal((result.Value as AircraftDTO).AircraftClass, aircraft.AircraftClass);
            Assert.Equal((result.Value as AircraftDTO).AircraftId, aircraft.AircraftId);
            Assert.Equal((result.Value as AircraftDTO).AircraftIdentifier, aircraft.AircraftIdentifier);
            Assert.Equal((result.Value as AircraftDTO).AircraftMake, aircraft.AircraftMake);
            Assert.Equal((result.Value as AircraftDTO).AircraftModel, aircraft.AircraftModel);
            Assert.Equal((result.Value as AircraftDTO).AircraftType, aircraft.AircraftType);
            Assert.Equal((result.Value as AircraftDTO).AircraftYear, aircraft.AircraftYear);
            Assert.Equal((result.Value as AircraftDTO).ChangedOn, aircraft.ChangedOn);
            Assert.Equal((result.Value as AircraftDTO).CreatedOn, aircraft.CreatedOn);
            Assert.Equal((result.Value as AircraftDTO).EngineType, aircraft.EngineType);
            Assert.Equal((result.Value as AircraftDTO).GearType, aircraft.GearType);
            Assert.Equal((result.Value as AircraftDTO).IsComplex, aircraft.IsComplex);
            Assert.Equal((result.Value as AircraftDTO).IsHighPerformance, aircraft.IsHighPerformance);
            Assert.Equal((result.Value as AircraftDTO).IsPressurized, aircraft.IsPressurized);
            Assert.Equal((result.Value as AircraftDTO).Logbook, null);
            Assert.Equal((result.Value as AircraftDTO).LogbookId, aircraft.LogbookId);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/aircraft
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostAircraftTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostAircraft_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            // Act
            var result = _controller.PostAircraft(logbook.LogbookId, aircraft).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostAircraft_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostAircraft(logbook.LogbookId, aircraft).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostAircraft_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostAircraft(logbook.LogbookId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostAircraft_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostAircraft(logbook.LogbookId, aircraft).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/aircraft
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostAircraft_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostAircraft(logbook.LogbookId, aircraft).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetAircraftByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], logbook.LogbookId);
            Assert.Equal(result.RouteValues["aircraftId"], aircraft.AircraftId);
            Assert.IsType(typeof(AircraftDTO), result.Value);
            Assert.Equal((result.Value as AircraftDTO).AircraftCategory, aircraft.AircraftCategory);
            Assert.Equal((result.Value as AircraftDTO).AircraftClass, aircraft.AircraftClass);
            Assert.Equal((result.Value as AircraftDTO).AircraftId, aircraft.AircraftId);
            Assert.Equal((result.Value as AircraftDTO).AircraftIdentifier, aircraft.AircraftIdentifier);
            Assert.Equal((result.Value as AircraftDTO).AircraftMake, aircraft.AircraftMake);
            Assert.Equal((result.Value as AircraftDTO).AircraftModel, aircraft.AircraftModel);
            Assert.Equal((result.Value as AircraftDTO).AircraftType, aircraft.AircraftType);
            Assert.Equal((result.Value as AircraftDTO).AircraftYear, aircraft.AircraftYear);
            Assert.Equal((result.Value as AircraftDTO).ChangedOn, aircraft.ChangedOn);
            Assert.Equal((result.Value as AircraftDTO).CreatedOn, aircraft.CreatedOn);
            Assert.Equal((result.Value as AircraftDTO).EngineType, aircraft.EngineType);
            Assert.Equal((result.Value as AircraftDTO).GearType, aircraft.GearType);
            Assert.Equal((result.Value as AircraftDTO).IsComplex, aircraft.IsComplex);
            Assert.Equal((result.Value as AircraftDTO).IsHighPerformance, aircraft.IsHighPerformance);
            Assert.Equal((result.Value as AircraftDTO).IsPressurized, aircraft.IsPressurized);
            Assert.Equal((result.Value as AircraftDTO).Logbook, null);
            Assert.Equal((result.Value as AircraftDTO).LogbookId, aircraft.LogbookId);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutAircraftTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutAircraft_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            // Act
            var result = _controller.PutAircraft(aircraft.LogbookId, aircraft.AircraftId, aircraft).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutAircraft_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutAircraft(aircraft.LogbookId, aircraft.AircraftId, aircraft).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutAircraft_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutAircraft(aircraft.LogbookId, aircraft.AircraftId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutAircraft_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutAircraft(aircraft.LogbookId, aircraft.AircraftId, aircraft).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutAircraft_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutAircraft(aircraft.LogbookId, aircraft.AircraftId, aircraft).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutAircraft_ShouldReturnNoContentOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutAircraft(aircraft.LogbookId, aircraft.AircraftId, aircraft).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteAircraftTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteAircraft_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            // Act
            var result = _controller.DeleteAircraft(aircraft.LogbookId, aircraft.AircraftId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteAircraft_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteAircraft(aircraft.LogbookId, aircraft.AircraftId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteAircraft_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteAircraft(aircraft.LogbookId, aircraft.AircraftId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/aircraft/{aircraftId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteAircraft_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Aircraft aircraft = MockDataFactory.CreateMockAircraft(userId, 1, logbook.LogbookId);
            _apiDbContext.Aircraft.Add(aircraft);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(aircraft).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteAircraft(aircraft.LogbookId, aircraft.AircraftId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(AircraftDTO), result.Value);
            Assert.Equal((result.Value as AircraftDTO).AircraftCategory, aircraft.AircraftCategory);
            Assert.Equal((result.Value as AircraftDTO).AircraftClass, aircraft.AircraftClass);
            Assert.Equal((result.Value as AircraftDTO).AircraftId, aircraft.AircraftId);
            Assert.Equal((result.Value as AircraftDTO).AircraftIdentifier, aircraft.AircraftIdentifier);
            Assert.Equal((result.Value as AircraftDTO).AircraftMake, aircraft.AircraftMake);
            Assert.Equal((result.Value as AircraftDTO).AircraftModel, aircraft.AircraftModel);
            Assert.Equal((result.Value as AircraftDTO).AircraftType, aircraft.AircraftType);
            Assert.Equal((result.Value as AircraftDTO).AircraftYear, aircraft.AircraftYear);
            Assert.Equal((result.Value as AircraftDTO).ChangedOn, aircraft.ChangedOn);
            Assert.Equal((result.Value as AircraftDTO).CreatedOn, aircraft.CreatedOn);
            Assert.Equal((result.Value as AircraftDTO).EngineType, aircraft.EngineType);
            Assert.Equal((result.Value as AircraftDTO).GearType, aircraft.GearType);
            Assert.Equal((result.Value as AircraftDTO).IsComplex, aircraft.IsComplex);
            Assert.Equal((result.Value as AircraftDTO).IsHighPerformance, aircraft.IsHighPerformance);
            Assert.Equal((result.Value as AircraftDTO).IsPressurized, aircraft.IsPressurized);
            Assert.Equal((result.Value as AircraftDTO).Logbook, null);
            Assert.Equal((result.Value as AircraftDTO).LogbookId, aircraft.LogbookId);
        }

        #endregion
    }
}
