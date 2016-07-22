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
    public class CertificatesControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<CertificatesController> _logger;
        private CertificatesController _controller;

        /// <summary>
        /// Constructs a new PilotsControllerTest object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and PilotsController
        /// objects for every test.
        /// </summary>
        public CertificatesControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<CertificatesController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new CertificatesController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/certificates
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllCertificatesTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAllCertificates_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            // Act
            var result = _controller.GetAllCertificates(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAllCertificates_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAllCertificates(logbook.LogbookId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates
        /// returns a 200 Ok response and an empty list when the requested resources are not found.
        /// </summary>
        [Fact]
        public void GetAllCertificates_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllCertificates(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(List<CertificateDTO>), result.Value);
            Assert.Equal((result.Value as List<CertificateDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllCertificates_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;
            Certificate certificate2 = MockDataFactory.CreateMockCertificate(userId, 2, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate2).State = EntityState.Detached;
            Certificate certificate3 = MockDataFactory.CreateMockCertificate(userId, 3, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllCertificates(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<CertificateDTO>));
            Assert.Equal((result.Value as List<CertificateDTO>).Count, 3);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates
        /// returns a 200 OK response and the appropriate resource when it exists, including related ratings
        /// </summary>
        public void GetAllCertificates_ShouldReturnRatingResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, 0, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;
            Rating rating2 = MockDataFactory.CreateMockRating(userId, 2, 0, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating2).State = EntityState.Detached;
            Rating rating3 = MockDataFactory.CreateMockRating(userId, 3, 0, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating3).State = EntityState.Detached;
            List<Rating> ratingList = new List<Rating> { rating1, rating2, rating3 };
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId, ratingList);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllCertificates(logbook.LogbookId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<CertificateDTO>));
            Assert.Equal((result.Value as List<CertificateDTO>).Count, 1);
            Assert.NotNull((result.Value as List<CertificateDTO>)[0].Ratings);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.Count, 3);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).Certificate, null);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).CertificateId, rating1.CertificateId);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).ChangedOn, rating1.ChangedOn);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).CreatedOn, rating1.CreatedOn);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).RatingDate, rating1.RatingDate);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).RatingId, rating1.RatingId);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).RatingType, rating1.RatingType);
            Assert.Equal((result.Value as List<CertificateDTO>)[0].Ratings.ElementAt(0).Remarks, rating1.Remarks);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetCertificateTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetCertificate_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            // Act
            var result = _controller.GetCertificate(certificate1.LogbookId, certificate1.CertificateId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetCertificate_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetCertificate(certificate1.LogbookId, certificate1.CertificateId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetCertificate_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetCertificate(certificate1.LogbookId, certificate1.CertificateId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetCertificate_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetCertificate(certificate1.LogbookId, certificate1.CertificateId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull((result.Value as CertificateDTO));
            Assert.Equal((result.Value as CertificateDTO).CertificateDate, certificate1.CertificateDate);
            Assert.Equal((result.Value as CertificateDTO).CertificateId, certificate1.CertificateId);
            Assert.Equal((result.Value as CertificateDTO).CertificateNumber, certificate1.CertificateNumber);
            Assert.Equal((result.Value as CertificateDTO).CertificateType, certificate1.CertificateType);
            Assert.Equal((result.Value as CertificateDTO).ChangedOn, certificate1.ChangedOn);
            Assert.Equal((result.Value as CertificateDTO).CreatedOn, certificate1.CreatedOn);
            Assert.Equal((result.Value as CertificateDTO).ExpirationDate, certificate1.ExpirationDate);
            Assert.Equal((result.Value as CertificateDTO).Logbook, null);
            Assert.Equal((result.Value as CertificateDTO).LogbookId, certificate1.LogbookId);
            Assert.NotNull((result.Value as CertificateDTO).Ratings);
            Assert.Equal((result.Value as CertificateDTO).Ratings.Count, 0);
            Assert.Equal((result.Value as CertificateDTO).Remarks, certificate1.Remarks);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 200 OK response and the appropriate resource when it exists, including related ratings
        /// </summary>
        public void GetCertificate_ShouldReturnRatingResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, 0, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;
            Rating rating2 = MockDataFactory.CreateMockRating(userId, 2, 0, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating2).State = EntityState.Detached;
            Rating rating3 = MockDataFactory.CreateMockRating(userId, 3, 0, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating3).State = EntityState.Detached;
            List<Rating> ratingList = new List<Rating> { rating1, rating2, rating3 };
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId, ratingList);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetCertificate(certificate1.LogbookId, certificate1.CertificateId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull((result.Value as CertificateDTO));
            Assert.NotNull((result.Value as CertificateDTO).Ratings);
            Assert.Equal((result.Value as CertificateDTO).Ratings.Count, 3);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).Certificate, null);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).CertificateId, rating1.CertificateId);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).ChangedOn, rating1.ChangedOn);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).CreatedOn, rating1.CreatedOn);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).RatingDate, rating1.RatingDate);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).RatingId, rating1.RatingId);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).RatingType, rating1.RatingType);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).Remarks, rating1.Remarks);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostCertificateTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostCertificate_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            // Act
            var result = _controller.PostCertificate(certificate1.LogbookId, certificate1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostCertificate_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostCertificate(certificate1.LogbookId, certificate1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostCertificate_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostCertificate(certificate1.LogbookId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostCertificate_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostCertificate(certificate1.LogbookId, certificate1).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostCertificate_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostCertificate(certificate1.LogbookId, certificate1).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetCertificateByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], certificate1.LogbookId);
            Assert.Equal(result.RouteValues["certificateId"], certificate1.CertificateId);
            Assert.IsType(typeof(CertificateDTO), result.Value);
            Assert.Equal((result.Value as CertificateDTO).CertificateDate, certificate1.CertificateDate);
            Assert.Equal((result.Value as CertificateDTO).CertificateId, certificate1.CertificateId);
            Assert.Equal((result.Value as CertificateDTO).CertificateNumber, certificate1.CertificateNumber);
            Assert.Equal((result.Value as CertificateDTO).CertificateType, certificate1.CertificateType);
            Assert.Equal((result.Value as CertificateDTO).ChangedOn, certificate1.ChangedOn);
            Assert.Equal((result.Value as CertificateDTO).CreatedOn, certificate1.CreatedOn);
            Assert.Equal((result.Value as CertificateDTO).ExpirationDate, certificate1.ExpirationDate);
            Assert.Equal((result.Value as CertificateDTO).Logbook, null);
            Assert.Equal((result.Value as CertificateDTO).LogbookId, certificate1.LogbookId);
            Assert.Equal((result.Value as CertificateDTO).Ratings.Count, 0);
            Assert.Equal((result.Value as CertificateDTO).Remarks, certificate1.Remarks);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 204 Created At response, the same resource on a valid request, along with associated rating
        /// </summary>
        [Fact]
        public void PostCertificate_ShouldReturnRatingResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1);
            List<Rating> ratingList = new List<Rating> { rating1 };
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId, ratingList);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostCertificate(certificate1.LogbookId, certificate1).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetCertificateByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], certificate1.LogbookId);
            Assert.Equal(result.RouteValues["certificateId"], certificate1.CertificateId);
            Assert.IsType(typeof(CertificateDTO), result.Value);
            Assert.NotNull((result.Value as CertificateDTO).Ratings);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).Certificate, null);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).CertificateId, rating1.CertificateId);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).ChangedOn, rating1.ChangedOn);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).CreatedOn, rating1.CreatedOn);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).RatingDate, rating1.RatingDate);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).RatingId, rating1.RatingId);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).RatingType, rating1.RatingType);
            Assert.Equal((result.Value as CertificateDTO).Ratings.ElementAt(0).Remarks, rating1.Remarks);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutCertificateTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutCertificate_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            // Act
            var result = _controller.PutCertificate(certificate1.LogbookId, certificate1.CertificateId, certificate1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutCertificate_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutCertificate(certificate1.LogbookId, certificate1.CertificateId, certificate1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutCertificate_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutCertificate(certificate1.LogbookId, certificate1.CertificateId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutCertificate_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutCertificate(certificate1.LogbookId, certificate1.CertificateId, certificate1).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutCertificate_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutCertificate(certificate1.LogbookId, certificate1.CertificateId, certificate1).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutCertificate_ShouldReturnNoContentOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutCertificate(certificate1.LogbookId, certificate1.CertificateId, certificate1).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteCertificateTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteCertificate_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            // Act
            var result = _controller.DeleteCertificate(certificate1.LogbookId, certificate1.CertificateId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteCertificate_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteCertificate(certificate1.LogbookId, certificate1.CertificateId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteCertificate_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteCertificate(certificate1.LogbookId, certificate1.CertificateId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteCertificate_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate1 = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteCertificate(certificate1.LogbookId, certificate1.CertificateId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(CertificateDTO), result.Value);
            Assert.Equal((result.Value as CertificateDTO).CertificateDate, certificate1.CertificateDate);
            Assert.Equal((result.Value as CertificateDTO).CertificateId, certificate1.CertificateId);
            Assert.Equal((result.Value as CertificateDTO).CertificateNumber, certificate1.CertificateNumber);
            Assert.Equal((result.Value as CertificateDTO).CertificateType, certificate1.CertificateType);
            Assert.Equal((result.Value as CertificateDTO).ChangedOn, certificate1.ChangedOn);
            Assert.Equal((result.Value as CertificateDTO).CreatedOn, certificate1.CreatedOn);
            Assert.Equal((result.Value as CertificateDTO).ExpirationDate, certificate1.ExpirationDate);
            Assert.Equal((result.Value as CertificateDTO).Logbook, null);
            Assert.Equal((result.Value as CertificateDTO).LogbookId, certificate1.LogbookId);
            Assert.NotNull((result.Value as CertificateDTO).Ratings);
            Assert.Equal((result.Value as CertificateDTO).Ratings.Count, 0);
            Assert.Equal((result.Value as CertificateDTO).Remarks, certificate1.Remarks);
        }

        #endregion
    }
}
