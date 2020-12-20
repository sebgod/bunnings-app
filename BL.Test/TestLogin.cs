using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;

namespace BL.Test
{
    class TestLogin
    {
        [Test]
        public void TestThatSubmittingAnUrlWillTriggerALoginIfNoSessionIsGiven()
        {
            const string validApiKey = "this is a valid key";
            const string validSessionKey = "abc123xyz";
            var someTestUri = new Uri("http://some.image.test/file.fits");

            var options = new PlateSolverOptions {
                ApiKey = validApiKey
            };
            var mockOptions = new Mock<IOptions<PlateSolverOptions>>();
            mockOptions.Setup(p => p.Value).Returns(options).Verifiable();

            var mockHttpMessageHandler = new MockHttpMessageHandler();

            mockHttpMessageHandler
                .Expect(HttpMethod.Post, "/api/login")
                .WithFormData("request-json", JsonSerializer.Serialize(new { apikey = validApiKey }))
                .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(new { status = "success", message = "", session = validSessionKey }));

            mockHttpMessageHandler
                .Expect(HttpMethod.Post, "/api/url_upload")
                .WithFormData("request-json", JsonSerializer.Serialize(new { session = validSessionKey, url = someTestUri.ToString() }))
                .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(new { status = "success", subid = 1, hash = "code" }));

            var mockClient = mockHttpMessageHandler.ToHttpClient();
            mockClient.BaseAddress = new Uri("http://nova.astrometry.test");
            async Task sut() => await new NovaNetPlateSolverService(mockClient, mockOptions.Object).BlindSolveImageUriAsync(someTestUri);

            Assert.That(sut, Throws.Nothing);

            mockOptions.Verify();
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void TestThatSubmittingAnUrlWillNotTriggerALoginIfSessionIsGiven()
        {
            const string validApiKey = "this is a valid key";
            const string validSessionKey = "abc123xyz";
            var someTestUri = new Uri("http://some.image.test/file.fits");

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var options = new PlateSolverOptions {
                ApiKey = validApiKey
            };
            var mockOptions = new Mock<IOptions<PlateSolverOptions>>();
            mockOptions.Setup(p => p.Value).Returns(options);

            mockHttpMessageHandler
                .Expect(HttpMethod.Post, "/api/url_upload")
                .WithFormData("request-json", JsonSerializer.Serialize(new { session = validSessionKey, url = someTestUri.ToString() }))
                .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(new { status = "success", subid = 1, hash = "code" }));

            var mockClient = mockHttpMessageHandler.ToHttpClient();
            mockClient.BaseAddress = new Uri("http://nova.astrometry.test");
            async Task sut() => await new NovaNetPlateSolverService(mockClient, mockOptions.Object).BlindSolveImageUriAsync(someTestUri, validSessionKey);

            Assert.That(sut, Throws.Nothing);

            mockOptions.VerifyNoOtherCalls();
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }
    }
}