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
using System.Linq;
using Xunit;

namespace ESP.FlightBook.Tests.Api.Controllers
{
    [Collection("Controller Tests")]
    public class FlightsControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<FlightsController> _logger;
        private FlightsController _controller;

        /// <summary>
        /// Constructs a new FlightsControllerUnitTests object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and FlightsController
        /// objects for every test.
        /// </summary>
        public FlightsControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<FlightsController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new FlightsController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/flights
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllFlightsTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAllFlights_ShouldReturnForbiddenOnMissingIdentity()
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

            // Act
            var result = _controller.GetAllFlights(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAllFlights_ShouldReturnForbiddenOnInvalidIdentity()
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

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAllFlights(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights
        /// returns a 200 Ok response and an empty list when the requested resources are not found.
        /// </summary>
        [Fact]
        public void GetAllFlights_ShouldReturnNotFoundOnLookupFailure()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllFlights(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(List<FlightDTO>), result.Value);
            Assert.Equal((result.Value as List<FlightDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllFlights_ShouldReturnSameResourcesOnSuccess()
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
            Flight flight1 = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight1).State = EntityState.Detached;
            Flight flight2 = MockDataFactory.CreateMockFlight(userId, 2, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight2).State = EntityState.Detached;
            Flight flight3 = MockDataFactory.CreateMockFlight(userId, 3, aircraft.AircraftId, logbook.LogbookId);
            _apiDbContext.Flights.Add(flight3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllFlights(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<FlightDTO>));
            Assert.Equal((result.Value as List<FlightDTO>).Count, 3);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights
        /// returns a 200 OK response and the appropriate resource when it exists, including related ratings
        /// </summary>
        public void GetAllFlights_ShouldReturnApproachResourcesOnSuccess()
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
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;
            Approach approach2 = MockDataFactory.CreateMockApproach(userId, 2);
            _apiDbContext.Approaches.Add(approach2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach2).State = EntityState.Detached;
            Approach approach3 = MockDataFactory.CreateMockApproach(userId, 3);
            _apiDbContext.Approaches.Add(approach3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach3).State = EntityState.Detached;
            List<Approach> approachList = new List<Approach> { approach1, approach2, approach3 };
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId, approachList);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllFlights(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<FlightDTO>));
            Assert.Equal((result.Value as List<FlightDTO>).Count, 1);
            Assert.NotNull((result.Value as List<FlightDTO>)[0].Approaches);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.Count, 3);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).AirportCode, approach1.AirportCode);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).ApproachId, approach1.ApproachId);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).ApproachType, approach1.ApproachType);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).ChangedOn, approach1.ChangedOn);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).CreatedOn, approach1.CreatedOn);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).Flight, null);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).FlightId, approach1.FlightId);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).IsCircleToLand, approach1.IsCircleToLand);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).Remarks, approach1.Remarks);
            Assert.Equal((result.Value as List<FlightDTO>)[0].Approaches.ElementAt(0).Runway, approach1.Runway);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetFlightTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetFlight_ShouldReturnForbiddenOnMissingIdentity()
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

            // Act
            var result = _controller.GetFlight(flight.LogbookId, flight.FlightId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetFlight_ShouldReturnForbiddenOnInvalidIdentity()
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

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetFlight(flight.LogbookId, flight.FlightId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetFlight_ShouldReturnNotFoundOnLookupFailure()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetFlight(flight.LogbookId, flight.FlightId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetFlight_ShouldReturnSameResourceOnSuccess()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetFlight(flight.LogbookId, flight.FlightId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull((result.Value as FlightDTO));
            Assert.NotNull((result.Value as FlightDTO).Aircraft);
            Assert.Equal((result.Value as FlightDTO).AircraftId, flight.AircraftId);
            Assert.NotNull((result.Value as FlightDTO).Approaches);
            Assert.Equal((result.Value as FlightDTO).Approaches.Count, 0);
            Assert.Equal((result.Value as FlightDTO).ChangedOn, flight.ChangedOn);
            Assert.Equal((result.Value as FlightDTO).CreatedOn, flight.CreatedOn);
            Assert.Equal((result.Value as FlightDTO).DepartureCode, flight.DepartureCode);
            Assert.Equal((result.Value as FlightDTO).DestinationCode, flight.DestinationCode);
            Assert.Equal((result.Value as FlightDTO).FlightDate, flight.FlightDate);
            Assert.Equal((result.Value as FlightDTO).FlightId, flight.FlightId);
            Assert.Equal((result.Value as FlightDTO).FlightTimeActualInstrument, flight.FlightTimeActualInstrument);
            Assert.Equal((result.Value as FlightDTO).FlightTimeCrossCountry, flight.FlightTimeCrossCountry);
            Assert.Equal((result.Value as FlightDTO).FlightTimeDay, flight.FlightTimeDay);
            Assert.Equal((result.Value as FlightDTO).FlightTimeDual, flight.FlightTimeDual);
            Assert.Equal((result.Value as FlightDTO).FlightTimeNight, flight.FlightTimeNight);
            Assert.Equal((result.Value as FlightDTO).FlightTimePIC, flight.FlightTimePIC);
            Assert.Equal((result.Value as FlightDTO).FlightTimeSimulatedInstrument, flight.FlightTimeSimulatedInstrument);
            Assert.Equal((result.Value as FlightDTO).FlightTimeSolo, flight.FlightTimeSolo);
            Assert.Equal((result.Value as FlightDTO).FlightTimeTotal, flight.FlightTimeTotal);
            Assert.Equal((result.Value as FlightDTO).IsCheckRide, flight.IsCheckRide);
            Assert.Equal((result.Value as FlightDTO).IsFlightReview, flight.IsFlightReview);
            Assert.Equal((result.Value as FlightDTO).IsInstrumentProficiencyCheck, flight.IsInstrumentProficiencyCheck);
            Assert.Equal((result.Value as FlightDTO).LogbookId, flight.LogbookId);
            Assert.Equal((result.Value as FlightDTO).NumberOfHolds, flight.NumberOfHolds);
            Assert.Equal((result.Value as FlightDTO).NumberOfLandingsDay, flight.NumberOfLandingsDay);
            Assert.Equal((result.Value as FlightDTO).NumberOfLandingsNight, flight.NumberOfLandingsNight);
            Assert.Equal((result.Value as FlightDTO).Remarks, flight.Remarks);
            Assert.Equal((result.Value as FlightDTO).Route, flight.Route);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 200 OK response and the appropriate resource when it exists, including related approaches
        /// </summary>
        public void GetFlight_ShouldReturnApproachResourcesOnSuccess()
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
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1);
            _apiDbContext.Approaches.Add(approach1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach1).State = EntityState.Detached;
            Approach approach2 = MockDataFactory.CreateMockApproach(userId, 2);
            _apiDbContext.Approaches.Add(approach2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach2).State = EntityState.Detached;
            Approach approach3 = MockDataFactory.CreateMockApproach(userId, 3);
            _apiDbContext.Approaches.Add(approach3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(approach3).State = EntityState.Detached;
            List<Approach> approachList = new List<Approach> { approach1, approach2, approach3 };
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId, approachList);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetFlight(flight.LogbookId, flight.FlightId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull((result.Value as FlightDTO));
            Assert.NotNull((result.Value as FlightDTO).Approaches);
            Assert.Equal((result.Value as FlightDTO).Approaches.Count, 3);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).AirportCode, approach1.AirportCode);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).ApproachId, approach1.ApproachId);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).ApproachType, approach1.ApproachType);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).ChangedOn, approach1.ChangedOn);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).CreatedOn, approach1.CreatedOn);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).Flight, null);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).FlightId, approach1.FlightId);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).IsCircleToLand, approach1.IsCircleToLand);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).Remarks, approach1.Remarks);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).Runway, approach1.Runway);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/flights
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostFlightTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostFlight_ShouldReturnForbiddenOnMissingIdentity()
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

            // Act
            var result = _controller.PostFlight(flight.LogbookId, flight).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostFlight_ShouldReturnForbiddenOnInvalidIdentity()
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

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostFlight(flight.LogbookId, flight).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostFlight_ShouldReturnBadRequestOnNullInput()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostFlight(flight.LogbookId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostFlight_ShouldReturnBadRequestOnInvalidModelState()
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

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostFlight(flight.LogbookId, flight).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostFlight_ShouldReturnSameResourceOnSuccess()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostFlight(flight.LogbookId, flight).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetFlightByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], flight.LogbookId);
            Assert.Equal(result.RouteValues["flightid"], flight.FlightId);
            Assert.IsType(typeof(FlightDTO), result.Value);
            Assert.Null((result.Value as FlightDTO).Aircraft);
            Assert.Equal((result.Value as FlightDTO).AircraftId, flight.AircraftId);
            Assert.NotNull((result.Value as FlightDTO).Approaches);
            Assert.Equal((result.Value as FlightDTO).Approaches.Count, 0);
            Assert.Equal((result.Value as FlightDTO).ChangedOn, flight.ChangedOn);
            Assert.Equal((result.Value as FlightDTO).CreatedOn, flight.CreatedOn);
            Assert.Equal((result.Value as FlightDTO).DepartureCode, flight.DepartureCode);
            Assert.Equal((result.Value as FlightDTO).DestinationCode, flight.DestinationCode);
            Assert.Equal((result.Value as FlightDTO).FlightDate, flight.FlightDate);
            Assert.Equal((result.Value as FlightDTO).FlightId, flight.FlightId);
            Assert.Equal((result.Value as FlightDTO).FlightTimeActualInstrument, flight.FlightTimeActualInstrument);
            Assert.Equal((result.Value as FlightDTO).FlightTimeCrossCountry, flight.FlightTimeCrossCountry);
            Assert.Equal((result.Value as FlightDTO).FlightTimeDay, flight.FlightTimeDay);
            Assert.Equal((result.Value as FlightDTO).FlightTimeDual, flight.FlightTimeDual);
            Assert.Equal((result.Value as FlightDTO).FlightTimeNight, flight.FlightTimeNight);
            Assert.Equal((result.Value as FlightDTO).FlightTimePIC, flight.FlightTimePIC);
            Assert.Equal((result.Value as FlightDTO).FlightTimeSimulatedInstrument, flight.FlightTimeSimulatedInstrument);
            Assert.Equal((result.Value as FlightDTO).FlightTimeSolo, flight.FlightTimeSolo);
            Assert.Equal((result.Value as FlightDTO).FlightTimeTotal, flight.FlightTimeTotal);
            Assert.Equal((result.Value as FlightDTO).IsCheckRide, flight.IsCheckRide);
            Assert.Equal((result.Value as FlightDTO).IsFlightReview, flight.IsFlightReview);
            Assert.Equal((result.Value as FlightDTO).IsInstrumentProficiencyCheck, flight.IsInstrumentProficiencyCheck);
            Assert.Equal((result.Value as FlightDTO).LogbookId, flight.LogbookId);
            Assert.Equal((result.Value as FlightDTO).NumberOfHolds, flight.NumberOfHolds);
            Assert.Equal((result.Value as FlightDTO).NumberOfLandingsDay, flight.NumberOfLandingsDay);
            Assert.Equal((result.Value as FlightDTO).NumberOfLandingsNight, flight.NumberOfLandingsNight);
            Assert.Equal((result.Value as FlightDTO).Remarks, flight.Remarks);
            Assert.Equal((result.Value as FlightDTO).Route, flight.Route);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/flights
        /// returns a 204 Created At response, the same resource on a valid request, along with associated approach
        /// </summary>
        [Fact]
        public void PostFlight_ShouldReturnApproachResourceOnSuccess()
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
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1);
            List<Approach> approachList = new List<Approach> { approach1 };
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId, approachList);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostFlight(flight.LogbookId, flight).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetFlightByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], flight.LogbookId);
            Assert.Equal(result.RouteValues["flightid"], flight.FlightId);
            Assert.IsType(typeof(FlightDTO), result.Value);
            Assert.NotNull((result.Value as FlightDTO).Approaches);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).AirportCode, approach1.AirportCode);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).ApproachId, approach1.ApproachId);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).ApproachType, approach1.ApproachType);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).ChangedOn, approach1.ChangedOn);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).CreatedOn, approach1.CreatedOn);
            Assert.Null((result.Value as FlightDTO).Approaches.ElementAt(0).Flight);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).FlightId, approach1.FlightId);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).IsCircleToLand, approach1.IsCircleToLand);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).Remarks, approach1.Remarks);
            Assert.Equal((result.Value as FlightDTO).Approaches.ElementAt(0).Runway, approach1.Runway);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutFlightTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnForbiddenOnMissingIdentity()
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

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, flight).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnForbiddenOnIncorrectIdentity()
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

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, flight).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnBadRequestOnNullInput()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnBadRequestOnInvalidModelState()
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

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, flight).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnNotFoundOnLookupFailure()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, flight).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnNoContentOnSuccess()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, flight).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated including
        /// approaches.
        /// </summary>
        [Fact]
        public void PutFlight_ShouldReturnNoContentOnSuccessWithApproaches()
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
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1);
            Approach approach2 = MockDataFactory.CreateMockApproach(userId, 2);
            Approach approach3 = MockDataFactory.CreateMockApproach(userId, 3);
            List<Approach> approachList = new List<Approach> { approach1, approach2, approach3 };
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId, approachList);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutFlight(flight.LogbookId, flight.FlightId, flight).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteFlightTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteFlight_ShouldReturnForbiddenOnMissingIdentity()
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

            // Act
            var result = _controller.DeleteFlight(flight.LogbookId, flight.FlightId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteFlight_ShouldReturnForbiddenOnIncorrectIdentity()
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

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteFlight(flight.LogbookId, flight.FlightId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteFlight_ShouldReturnNotFoundOnLookupFailure()
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

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteFlight(flight.LogbookId, flight.FlightId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/flights/{flightId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteFlight_ShouldReturnSameResourceOnSuccess()
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
            Approach approach1 = MockDataFactory.CreateMockApproach(userId, 1);
            Approach approach2 = MockDataFactory.CreateMockApproach(userId, 2);
            Approach approach3 = MockDataFactory.CreateMockApproach(userId, 3);
            List<Approach> approachList = new List<Approach> { approach1, approach2, approach3 };
            Flight flight = MockDataFactory.CreateMockFlight(userId, 1, aircraft.AircraftId, logbook.LogbookId, approachList);
            _apiDbContext.Flights.Add(flight);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(flight).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteFlight(flight.LogbookId, flight.FlightId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull((result.Value as FlightDTO));
            Assert.NotNull((result.Value as FlightDTO).Aircraft);
            Assert.Equal((result.Value as FlightDTO).AircraftId, flight.AircraftId);
            Assert.NotNull((result.Value as FlightDTO).Approaches);
            Assert.Equal((result.Value as FlightDTO).Approaches.Count, 3);
            Assert.Equal((result.Value as FlightDTO).ChangedOn, flight.ChangedOn);
            Assert.Equal((result.Value as FlightDTO).CreatedOn, flight.CreatedOn);
            Assert.Equal((result.Value as FlightDTO).DepartureCode, flight.DepartureCode);
            Assert.Equal((result.Value as FlightDTO).DestinationCode, flight.DestinationCode);
            Assert.Equal((result.Value as FlightDTO).FlightDate, flight.FlightDate);
            Assert.Equal((result.Value as FlightDTO).FlightId, flight.FlightId);
            Assert.Equal((result.Value as FlightDTO).FlightTimeActualInstrument, flight.FlightTimeActualInstrument);
            Assert.Equal((result.Value as FlightDTO).FlightTimeCrossCountry, flight.FlightTimeCrossCountry);
            Assert.Equal((result.Value as FlightDTO).FlightTimeDay, flight.FlightTimeDay);
            Assert.Equal((result.Value as FlightDTO).FlightTimeDual, flight.FlightTimeDual);
            Assert.Equal((result.Value as FlightDTO).FlightTimeNight, flight.FlightTimeNight);
            Assert.Equal((result.Value as FlightDTO).FlightTimePIC, flight.FlightTimePIC);
            Assert.Equal((result.Value as FlightDTO).FlightTimeSimulatedInstrument, flight.FlightTimeSimulatedInstrument);
            Assert.Equal((result.Value as FlightDTO).FlightTimeSolo, flight.FlightTimeSolo);
            Assert.Equal((result.Value as FlightDTO).FlightTimeTotal, flight.FlightTimeTotal);
            Assert.Equal((result.Value as FlightDTO).IsCheckRide, flight.IsCheckRide);
            Assert.Equal((result.Value as FlightDTO).IsFlightReview, flight.IsFlightReview);
            Assert.Equal((result.Value as FlightDTO).IsInstrumentProficiencyCheck, flight.IsInstrumentProficiencyCheck);
            Assert.Equal((result.Value as FlightDTO).LogbookId, flight.LogbookId);
            Assert.Equal((result.Value as FlightDTO).NumberOfHolds, flight.NumberOfHolds);
            Assert.Equal((result.Value as FlightDTO).NumberOfLandingsDay, flight.NumberOfLandingsDay);
            Assert.Equal((result.Value as FlightDTO).NumberOfLandingsNight, flight.NumberOfLandingsNight);
            Assert.Equal((result.Value as FlightDTO).Remarks, flight.Remarks);
            Assert.Equal((result.Value as FlightDTO).Route, flight.Route);
        }

        #endregion
    }
}
