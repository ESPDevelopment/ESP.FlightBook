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
    public class RatingsControllerUnitTests : ControllerUnitTests, IDisposable
    {
        private ApplicationDbContext _apiDbContext;
        private ILogger<RatingsController> _logger;
        private RatingsController _controller;

        /// <summary>
        /// Constructs a new RatingsControllerUnitTests object.  This constructor is called
        /// for every test thus it is used to create the mock ApiDbContext and RatingsController
        /// objects for every test.
        /// </summary>
        public RatingsControllerUnitTests()
        {
            // Construct a logger
            var factory = new LoggerFactory().AddConsole();
            _logger = new Logger<RatingsController>(factory);

            // Construct a new mock ApiDbContext as an in-memory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _apiDbContext = new ApplicationDbContext(optionsBuilder.Options);
            _apiDbContext.Database.EnsureDeleted();
            _apiDbContext.Database.EnsureCreated();

            // Construct a new controller
            _controller = new RatingsController(_apiDbContext, _logger);
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
        // GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetAllRatingsTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetAllRatings_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            // Act
            var result = _controller.GetAllRatings(certificate.LogbookId, certificate.CertificateId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetAllRatings_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetAllRatings(certificate.LogbookId, certificate.CertificateId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 200 Ok response and an empty list when the requested resources are not found.
        /// </summary>
        [Fact]
        public void GetAllRatings_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllRatings(certificate.LogbookId, certificate.CertificateId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(List<RatingDTO>), result.Value);
            Assert.Equal((result.Value as List<RatingDTO>).Count, 0);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetAllRatings_ShouldReturnSameResourcesOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;
            Rating rating2 = MockDataFactory.CreateMockRating(userId, 2, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating2);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating2).State = EntityState.Detached;
            Rating rating3 = MockDataFactory.CreateMockRating(userId, 3, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating3);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating3).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetAllRatings(certificate.LogbookId, certificate.CertificateId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.NotNull((result.Value as List<RatingDTO>));
            Assert.Equal((result.Value as List<RatingDTO>).Count, 3);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region GetRatingTests

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void GetRating_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            // Act
            var result = _controller.GetRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void GetRating_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.GetRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal((result as StatusCodeResult).StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 404 Not Found response when the requested resource is not found.
        /// </summary>
        [Fact]
        public void GetRating_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request GET api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 200 OK response and the appropriate resource when it exists.
        /// </summary>
        [Fact]
        public void GetRating_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.GetRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(RatingDTO), result.Value);
            Assert.Null((result.Value as RatingDTO).Certificate);
            Assert.Equal((result.Value as RatingDTO).CertificateId, rating1.CertificateId);
            Assert.Equal((result.Value as RatingDTO).ChangedOn, rating1.ChangedOn);
            Assert.Equal((result.Value as RatingDTO).CreatedOn, rating1.CreatedOn);
            Assert.Equal((result.Value as RatingDTO).RatingDate, rating1.RatingDate);
            Assert.Equal((result.Value as RatingDTO).RatingId, rating1.RatingId);
            Assert.Equal((result.Value as RatingDTO).RatingType, rating1.RatingType);
            Assert.Equal((result.Value as RatingDTO).Remarks, rating1.Remarks);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PostRatingTests

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PostRating_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            // Act
            var result = _controller.PostRating(certificate.LogbookId, rating1.CertificateId, rating1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 403 Forbidden response when the identity is not authorized.
        /// </summary>
        [Fact]
        public void PostRating_ShouldReturnForbiddenOnInvalidIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PostRating(certificate.LogbookId, rating1.CertificateId, rating1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PostRating_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostRating(certificate.LogbookId, rating1.CertificateId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 409 Bad Request response on invalid model state
        /// </summary>
        [Fact]
        public void PostRating_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ModelState.AddModelError("EmailAddress", "EmailAddess validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostRating(certificate.LogbookId, rating1.CertificateId, rating1).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request POST api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings
        /// returns a 204 Created At response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void PostRating_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PostRating(certificate.LogbookId, rating1.CertificateId, rating1).Result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(CreatedAtRouteResult), result);
            Assert.Equal(result.StatusCode, 201);
            Assert.Equal(result.RouteName, "GetRatingByIdRoute");
            Assert.Equal(result.RouteValues["logbookId"], certificate.LogbookId);
            Assert.Equal(result.RouteValues["certificateId"], rating1.CertificateId);
            Assert.Equal(result.RouteValues["ratingId"], rating1.RatingId);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(RatingDTO), result.Value);
            Assert.Null((result.Value as RatingDTO).Certificate);
            Assert.Equal((result.Value as RatingDTO).CertificateId, rating1.CertificateId);
            Assert.Equal((result.Value as RatingDTO).ChangedOn, rating1.ChangedOn);
            Assert.Equal((result.Value as RatingDTO).CreatedOn, rating1.CreatedOn);
            Assert.Equal((result.Value as RatingDTO).RatingDate, rating1.RatingDate);
            Assert.Equal((result.Value as RatingDTO).RatingId, rating1.RatingId);
            Assert.Equal((result.Value as RatingDTO).RatingType, rating1.RatingType);
            Assert.Equal((result.Value as RatingDTO).Remarks, rating1.Remarks);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region PutRatingTests

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void PutRating_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            // Act
            var result = _controller.PutRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId, rating1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void PutRating_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.PutRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId, rating1).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 409 Bad Request response on null input
        /// </summary>
        [Fact]
        public void PutRating_ShouldReturnBadRequestOnNullInput()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId, null).Result as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 409 Bad Request response on an invalid model state
        /// </summary>
        [Fact]
        public void PutRating_ShouldReturnBadRequestOnInvalidModelState()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            _controller.ModelState.AddModelError("Title", "Title validation error.");
            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId, rating1).Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(BadRequestObjectResult), result);
            Assert.NotNull(result.Value);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void PutRating_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId, rating1).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request PUT api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 204 No Content response when the requested resource is successfully updated
        /// </summary>
        [Fact]
        public void PutRating_ShouldReturnNoContentOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.PutRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId, rating1).Result as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NoContentResult), result);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region DeleteRatingTests

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 403 Forbidden response when the identity is missing.
        /// </summary>
        [Fact]
        public void DeleteRating_ShouldReturnForbiddenOnMissingIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            // Act
            var result = _controller.DeleteRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 403 Forbidden response when the caller is not authorized to access
        /// the resource.
        /// </summary>
        [Fact]
        public void DeleteRating_ShouldReturnForbiddenOnIncorrectIdentity()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            string invalidUserId = "6D888455-E62E-44EE-AEFB-239802E9CABD";
            _controller.ControllerContext.HttpContext.User = GetUser(invalidUserId);

            // Act
            var result = _controller.DeleteRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(StatusCodeResult), result);
            Assert.Equal(result.StatusCode, 403);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 404 Not Found response when the requested resource is not found
        /// </summary>
        [Fact]
        public void DeleteRating_ShouldReturnNotFoundOnLookupFailure()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(NotFoundResult), result);
        }

        /// <summary>
        /// This test checks that the request DELETE api/v1/logbooks/{logbookId:int}/certificates/{certificateId:int}/ratings/{ratingId:int}
        /// returns a 200 OK response and the same resource on a valid request
        /// </summary>
        [Fact]
        public void DeleteRating_ShouldReturnSameResourceOnSuccess()
        {
            // Arrange
            string userId = "49A4FF92-C0ED-471D-89F6-527BE7D72FA7";
            Logbook logbook = MockDataFactory.CreateMockLogbook(userId, 1);
            _apiDbContext.Logbooks.Add(logbook);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(logbook).State = EntityState.Detached;
            Certificate certificate = MockDataFactory.CreateMockCertificate(userId, 1, logbook.LogbookId);
            _apiDbContext.Certificates.Add(certificate);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(certificate).State = EntityState.Detached;
            Rating rating1 = MockDataFactory.CreateMockRating(userId, 1, certificate.CertificateId, logbook.LogbookId);
            _apiDbContext.Ratings.Add(rating1);
            _apiDbContext.SaveChanges();
            _apiDbContext.Entry(rating1).State = EntityState.Detached;

            _controller.ControllerContext.HttpContext.User = GetUser(userId);

            // Act
            var result = _controller.DeleteRating(certificate.LogbookId, rating1.CertificateId, rating1.RatingId).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.NotNull(result.Value);
            Assert.IsType(typeof(RatingDTO), result.Value);
            Assert.Null((result.Value as RatingDTO).Certificate);
            Assert.Equal((result.Value as RatingDTO).CertificateId, rating1.CertificateId);
            Assert.Equal((result.Value as RatingDTO).ChangedOn, rating1.ChangedOn);
            Assert.Equal((result.Value as RatingDTO).CreatedOn, rating1.CreatedOn);
            Assert.Equal((result.Value as RatingDTO).RatingDate, rating1.RatingDate);
            Assert.Equal((result.Value as RatingDTO).RatingId, rating1.RatingId);
            Assert.Equal((result.Value as RatingDTO).RatingType, rating1.RatingType);
            Assert.Equal((result.Value as RatingDTO).Remarks, rating1.Remarks);
        }

        #endregion
    }
}
