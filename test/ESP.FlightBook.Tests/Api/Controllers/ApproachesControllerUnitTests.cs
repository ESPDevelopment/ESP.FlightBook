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
    public class ApproachesControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<ApproachesController> _logger;
        private ApproachesController _controller;

        /// <summary>
        /// Constructs a new ApproachesControllerUnitTests object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and ApproachesController
        /// objects for every test.
        /// </summary>
        public ApproachesControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<ApproachesController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new ApproachesController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllApproachesTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAllApproaches_ShouldReturnForbiddenOnMissingIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            // Act
            var result = _controller.GetAllApproaches(logbook.LogbookId, flight.FlightId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAllApproaches_ShouldReturnForbiddenOnInvalidIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAllApproaches(logbook.LogbookId, flight.FlightId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches
        /// returns a 200 Ok response and an empty list when the requested resources are not found.
        /// </summary>
        [Fact]
        public void GetAllApproaches_ShouldReturnNotFoundOnLookupFailure()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllApproaches(logbook.LogbookId, flight.FlightId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(List<ApproachDTO>), result.Value);
            Assert.Equal((result.Value as List<ApproachDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllApproaches_ShouldReturnSameResourcesOnSuccess()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;
            Approach approach2 = MockDataFactory.CreateMockApproach(userId, 2, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach2).State = EntityState.Detached;
            Approach approach3 = MockDataFactory.CreateMockApproach(userId, 3, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllApproaches(logbook.LogbookId, flight.FlightId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<ApproachDTO>));
            Assert.Equal((result.Value as List<ApproachDTO>).Count, 3);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetApproachTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetApproach_ShouldReturnForbiddenOnMissingIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            // Act
            var result = _controller.GetApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetApproach_ShouldReturnForbiddenOnInvalidIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetApproach_ShouldReturnNotFoundOnLookupFailure()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetApproach_ShouldReturnSameResourceOnSuccess()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(ApproachDTO), result.Value);
            Assert.Equal((result.Value as ApproachDTO).AirportCode, approach1.AirportCode);
            Assert.Equal((result.Value as ApproachDTO).ApproachId, approach1.ApproachId);
            Assert.Equal((result.Value as ApproachDTO).ApproachType, approach1.ApproachType);
            Assert.Equal((result.Value as ApproachDTO).ChangedOn, approach1.ChangedOn);
            Assert.Equal((result.Value as ApproachDTO).CreatedOn, approach1.CreatedOn);
            Assert.Equal((result.Value as ApproachDTO).Flight, null);
            Assert.Equal((result.Value as ApproachDTO).FlightId, approach1.FlightId);
            Assert.Equal((result.Value as ApproachDTO).IsCircleToLand, approach1.IsCircleToLand);
            Assert.Equal((result.Value as ApproachDTO).Remarks, approach1.Remarks);
            Assert.Equal((result.Value as ApproachDTO).Runway, approach1.Runway);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostApproachTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostApproach_ShouldReturnForbiddenOnMissingIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            // Act
            var result = _controller.PostApproach(flight.LogbookId, approach1.FlightId, approach1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostApproach_ShouldReturnForbiddenOnInvalidIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostApproach(flight.LogbookId, approach1.FlightId, approach1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostApproach_ShouldReturnBadRequestOnNullInput()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostApproach(flight.LogbookId, approach1.FlightId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostApproach_ShouldReturnBadRequestOnInvalidModelState()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostApproach(flight.LogbookId, approach1.FlightId, approach1).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostApproach_ShouldReturnSameResourceOnSuccess()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostApproach(flight.LogbookId, approach1.FlightId, approach1).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetApproachByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], logbook.LogbookId);
            Assert.Equal(result.RouteValues["flightId"], approach1.FlightId);
            Assert.Equal(result.RouteValues["approachId"], approach1.ApproachId);
            Assert.IsType(typeof(ApproachDTO), result.Value);
            Assert.Equal((result.Value as ApproachDTO).AirportCode, approach1.AirportCode);
            Assert.Equal((result.Value as ApproachDTO).ApproachId, approach1.ApproachId);
            Assert.Equal((result.Value as ApproachDTO).ApproachType, approach1.ApproachType);
            Assert.Equal((result.Value as ApproachDTO).ChangedOn, approach1.ChangedOn);
            Assert.Equal((result.Value as ApproachDTO).CreatedOn, approach1.CreatedOn);
            Assert.Equal((result.Value as ApproachDTO).Flight, null);
            Assert.Equal((result.Value as ApproachDTO).FlightId, approach1.FlightId);
            Assert.Equal((result.Value as ApproachDTO).IsCircleToLand, approach1.IsCircleToLand);
            Assert.Equal((result.Value as ApproachDTO).Remarks, approach1.Remarks);
            Assert.Equal((result.Value as ApproachDTO).Runway, approach1.Runway);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutApproachTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutApproach_ShouldReturnForbiddenOnMissingIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            // Act
            var result = _controller.PutApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId, approach1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutApproach_ShouldReturnForbiddenOnIncorrectIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId, approach1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutApproach_ShouldReturnBadRequestOnNullInput()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutApproach_ShouldReturnBadRequestOnInvalidModelState()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId, approach1).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutApproach_ShouldReturnNotFoundOnLookupFailure()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId, approach1).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutApproach_ShouldReturnNoContentOnSuccess()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId, approach1).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteApproachTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteApproach_ShouldReturnForbiddenOnMissingIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            // Act
            var result = _controller.DeleteApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteApproach_ShouldReturnForbiddenOnIncorrectIdentity()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteApproach_ShouldReturnNotFoundOnLookupFailure()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}/approaches/{approachId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteApproach_ShouldReturnSameResourceOnSuccess()
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
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1, flight.FlightId, logbook.LogbookId);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteApproach(flight.LogbookId, approach1.FlightId, approach1.ApproachId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(ApproachDTO), result.Value);
            Assert.Equal((result.Value as ApproachDTO).AirportCode, approach1.AirportCode);
            Assert.Equal((result.Value as ApproachDTO).ApproachId, approach1.ApproachId);
            Assert.Equal((result.Value as ApproachDTO).ApproachType, approach1.ApproachType);
            Assert.Equal((result.Value as ApproachDTO).ChangedOn, approach1.ChangedOn);
            Assert.Equal((result.Value as ApproachDTO).CreatedOn, approach1.CreatedOn);
            Assert.Equal((result.Value as ApproachDTO).Flight, null);
            Assert.Equal((result.Value as ApproachDTO).FlightId, approach1.FlightId);
            Assert.Equal((result.Value as ApproachDTO).IsCircleToLand, approach1.IsCircleToLand);
            Assert.Equal((result.Value as ApproachDTO).Remarks, approach1.Remarks);
            Assert.Equal((result.Value as ApproachDTO).Runway, approach1.Runway);
        }

        #endregion
    }
}
