using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace BL.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestThatNovaNetPlateSolverServiceConstructorFailsIfNoHttpClientIsResolved()
        {
            var mockOptions = new Mock<IOptions<PlateSolverOptions>>();
            Assert.That(() => new NovaNetPlateSolverService(null as HttpClient, mockOptions.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void TestThatNovaNetPlateSolverServiceConstructorFailsIfNoOptionsAreResolved()
        {
            Assert.That(() => new NovaNetPlateSolverService(new HttpClient(), null), Throws.ArgumentNullException);
        }
    }
}