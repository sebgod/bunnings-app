using NUnit.Framework;
using Api.Controllers;
using Api.Models;
using Moq;
using Microsoft.Extensions.Logging;
using BL;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Net;

namespace Api.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestThatProvidingAnInvalidSubmissionIdIsABadRequest()
        {
            const int invalidId = -1;
            var mockLogger = new Mock<ILogger<PlateSolveController>>();
            var mockPlateSolverService = new Mock<IPlateSolverService>();

            var controller = new PlateSolveController(mockLogger.Object, mockPlateSolverService.Object);

            Task<IActionResult> sut() => controller.GetAsync(invalidId);

            var actionResult = await sut();
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.TryGetStatusCode(), Is.EqualTo(HttpStatusCode.BadRequest));

            mockPlateSolverService.Verify(p => p.GetJobsForSubmissionAsync(invalidId));
            mockPlateSolverService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task TestThatProvidingNoModelOnPostIsABadRequest()
        {
            var mockLogger = new Mock<ILogger<PlateSolveController>>();
            var mockPlateSolverService = new Mock<IPlateSolverService>();

            var controller = new PlateSolveController(mockLogger.Object, mockPlateSolverService.Object);

            Task<IActionResult> sut() => controller.PostAsync(default);

            var actionResult = await sut();
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.TryGetStatusCode(), Is.EqualTo(HttpStatusCode.BadRequest));

            mockPlateSolverService.VerifyNoOtherCalls();
        }

        [TestCase("this/is/not/absolute")]
        [TestCase("")]
        public async Task TestThatProvidingANonAbsoluteUriOnPostIsABadRequest(string uri)
        {
            var mockLogger = new Mock<ILogger<PlateSolveController>>();
            var mockPlateSolverService = new Mock<IPlateSolverService>();

            var controller = new PlateSolveController(mockLogger.Object, mockPlateSolverService.Object);

            Task<IActionResult> sut() => controller.PostAsync(new PlateSolveModel(new Uri(uri, UriKind.Relative)));

            var actionResult = await sut();
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.TryGetStatusCode(), Is.EqualTo(HttpStatusCode.BadRequest));

            mockPlateSolverService.VerifyNoOtherCalls();
        }

        [TestCase("http://apod.nasa.gov/apod/image/2011/M78_LDN1622_BarnardsLoop_SEP27_28_Oct15_final1024.jpg")]
        [TestCase("http://apod.nasa.gov/apod/image/1206/ldn673s_block1123.jpg")]
        public async Task TestThatProvidingAValidImageUriGivesASubmission(string uri)
        {
            const int subId = 1;
            const string newSessionKey = "a valid session key";
            var absImageUri = new Uri(uri, UriKind.Absolute);
            var mockLogger = new Mock<ILogger<PlateSolveController>>();
            var mockPlateSolverService = new Mock<IPlateSolverService>();
            mockPlateSolverService
                .Setup(p => p.BlindSolveImageUriAsync(It.Is<Uri>(v => v == absImageUri), It.IsAny<string>()))
                .ReturnsAsync((Uri imageUri, string sessionKey) => new SubmissionHandle(sessionKey ?? newSessionKey, subId))
                .Verifiable();

            var controller = new PlateSolveController(mockLogger.Object, mockPlateSolverService.Object);

            Task<IActionResult> sut() => controller.PostAsync(new PlateSolveModel(absImageUri));

            var actionResult = await sut();
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.TryGetStatusCode(), Is.EqualTo(HttpStatusCode.Accepted));

            mockPlateSolverService.Verify();
            mockPlateSolverService.VerifyNoOtherCalls();
        }
    }
}