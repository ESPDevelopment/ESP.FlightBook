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
    public class EndorsementsControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<EndorsementsController> _logger;
        private EndorsementsController _controller;

        /// <summary>
        /// Constructs a new PilotsControllerTest object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and PilotsController
        /// objects for every test.
        /// </summary>
        public EndorsementsControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<EndorsementsController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new EndorsementsController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/endorsements
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllEndorsementsTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAllEndorsements_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            // Act
            var result = _controller.GetAllEndorsements(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAllEndorsements_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAllEndorsements(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 200 Ok response and an empty list when the requested resources are not found.
        /// </summary>
        [Fact]
        public void GetAllEndorsements_ShouldReturnEmptyListOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllEndorsements(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(List<EndorsementDTO>), result.Value);
            Assert.Equal((result.Value as List<EndorsementDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllEndorsements_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement1 = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement1).State = EntityState.Detached;
            Endorsement endorsement2 = MockDataFactory.CreateMockEndorsement(userId, 2, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement2).State = EntityState.Detached;
            Endorsement endorsement3 = MockDataFactory.CreateMockEndorsement(userId, 3, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllEndorsements(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<EndorsementDTO>));
            Assert.Equal((result.Value as List<EndorsementDTO>).Count, 3);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetEndorsementTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetEndorsement_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            // Act
            var result = _controller.GetEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetEndorsement_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetEndorsement_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetEndorsement_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(EndorsementDTO), result.Value);
            Assert.Equal((result.Value as EndorsementDTO).CFIExpiration, endorsement.CFIExpiration);
            Assert.Equal((result.Value as EndorsementDTO).CFIName, endorsement.CFIName);
            Assert.Equal((result.Value as EndorsementDTO).CFINumber, endorsement.CFINumber);
            Assert.Equal((result.Value as EndorsementDTO).ChangedOn, endorsement.ChangedOn);
            Assert.Equal((result.Value as EndorsementDTO).CreatedOn, endorsement.CreatedOn);
            Assert.Equal((result.Value as EndorsementDTO).EndorsementDate, endorsement.EndorsementDate);
            Assert.Equal((result.Value as EndorsementDTO).EndorsementId, endorsement.EndorsementId);
            Assert.Equal((result.Value as EndorsementDTO).Logbook, null);
            Assert.Equal((result.Value as EndorsementDTO).LogbookId, endorsement.LogbookId);
            Assert.Equal((result.Value as EndorsementDTO).Text, endorsement.Text);
            Assert.Equal((result.Value as EndorsementDTO).Title, endorsement.Title);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/endorsements
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostEndorsementTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostEndorsement_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            // Act
            var result = _controller.PostEndorsement(logbook.LogbookId, endorsement).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostEndorsement_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostEndorsement(logbook.LogbookId, endorsement).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostEndorsement_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostEndorsement(logbook.LogbookId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostEndorsement_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostEndorsement(logbook.LogbookId, endorsement).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/endorsements
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostEndorsement_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostEndorsement(logbook.LogbookId, endorsement).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetEndorsementByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], endorsement.LogbookId);
            Assert.Equal(result.RouteValues["endorsementId"], endorsement.EndorsementId);
            Assert.IsType(typeof(EndorsementDTO), result.Value);
            Assert.Equal((result.Value as EndorsementDTO).CFIExpiration, endorsement.CFIExpiration);
            Assert.Equal((result.Value as EndorsementDTO).CFIName, endorsement.CFIName);
            Assert.Equal((result.Value as EndorsementDTO).CFINumber, endorsement.CFINumber);
            Assert.Equal((result.Value as EndorsementDTO).ChangedOn, endorsement.ChangedOn);
            Assert.Equal((result.Value as EndorsementDTO).CreatedOn, endorsement.CreatedOn);
            Assert.Equal((result.Value as EndorsementDTO).EndorsementDate, endorsement.EndorsementDate);
            Assert.Equal((result.Value as EndorsementDTO).EndorsementId, endorsement.EndorsementId);
            Assert.Equal((result.Value as EndorsementDTO).Logbook, null);
            Assert.Equal((result.Value as EndorsementDTO).LogbookId, endorsement.LogbookId);
            Assert.Equal((result.Value as EndorsementDTO).Text, endorsement.Text);
            Assert.Equal((result.Value as EndorsementDTO).Title, endorsement.Title);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutEndorsementTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutEndorsement_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            // Act
            var result = _controller.PutEndorsement(endorsement.LogbookId, endorsement.EndorsementId, endorsement).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutEndorsement_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutEndorsement(endorsement.LogbookId, endorsement.EndorsementId, endorsement).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutEndorsement_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutEndorsement(endorsement.LogbookId, endorsement.EndorsementId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutEndorsement_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutEndorsement(endorsement.LogbookId, endorsement.EndorsementId, endorsement).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutEndorsement_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutEndorsement(endorsement.LogbookId, endorsement.EndorsementId, endorsement).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutEndorsement_ShouldReturnNoContentOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutEndorsement(endorsement.LogbookId, endorsement.EndorsementId, endorsement).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteEndorsementTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteEndorsement_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            // Act
            var result = _controller.DeleteEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteEndorsement_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteEndorsement_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/endorsements/{endorsementId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteEndorsement_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Endorsement endorsement = MockDataFactory.CreateMockEndorsement(userId, 1, logbook.LogbookId);
            _apiDbContext.Endorsements.Add(endorsement);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(endorsement).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteEndorsement(endorsement.LogbookId, endorsement.EndorsementId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(EndorsementDTO), result.Value);
            Assert.Equal((result.Value as EndorsementDTO).CFIExpiration, endorsement.CFIExpiration);
            Assert.Equal((result.Value as EndorsementDTO).CFIName, endorsement.CFIName);
            Assert.Equal((result.Value as EndorsementDTO).CFINumber, endorsement.CFINumber);
            Assert.Equal((result.Value as EndorsementDTO).ChangedOn, endorsement.ChangedOn);
            Assert.Equal((result.Value as EndorsementDTO).CreatedOn, endorsement.CreatedOn);
            Assert.Equal((result.Value as EndorsementDTO).EndorsementDate, endorsement.EndorsementDate);
            Assert.Equal((result.Value as EndorsementDTO).EndorsementId, endorsement.EndorsementId);
            Assert.Equal((result.Value as EndorsementDTO).Logbook, null);
            Assert.Equal((result.Value as EndorsementDTO).LogbookId, endorsement.LogbookId);
            Assert.Equal((result.Value as EndorsementDTO).Text, endorsement.Text);
            Assert.Equal((result.Value as EndorsementDTO).Title, endorsement.Title);
        }

        #endregion
    }
}
