using System.Net.Http;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ChuckNorrisTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Should_respond_ok_with_joke_requests()
        {
            var mockHandler = new MockHttpMessageHandler();

            mockHandler
                .When("https://api.chucknorris.io/jokes/random")
                .Respond("application/json", "{'status' : 'OK'}");

            var result = new HttpClient(mockHandler).GetAsync("https://api.chucknorris.io/jokes/random").Result;

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);

        }
    }
}
