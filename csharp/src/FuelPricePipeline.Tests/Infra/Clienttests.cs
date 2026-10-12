using System.Net;
using FuelPricePipeline.Infra.Eia;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace FuelPricePipeline.Tests.Infra;

[TestClass]
public class ClientTests
{
    /// <summary>
    /// Builds a Client with a static HTTP call with no network connection.
    /// </summary>
    /// <param name="responseJson"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    private static Client CreateClient(string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson)
            });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(handler.Object));

        var logger = new Mock<ILogger<Client>>();

        return new Client("test-key", factory.Object, logger.Object);
    }

    /// <summary>
    /// Tests invalid areas.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetLatestDieselAsync_WithInvalidArea_ShouldThrowArgumentException()
    {
        var client = CreateClient("{}");

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await client.FetchLatestDieselAsync("INVALID"));
    }

    /// <summary>
    /// Tests that a valid response parses every field.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetLatestDieselAsync_WithValidResponse_ShouldParseAllFields()
    {
        const string json = """
        {
            "response": {
                "data": [
                    {
                        "period": "2026-09-28",
                        "duoarea": "NUS",
                        "area-name": "U.S.",
                        "product": "EPD2D",
                        "product-name": "No 2 Diesel",
                        "value": "3.744",
                        "units": "$/GAL"
                    }
                ]
            }
        }
        """;

        var client = CreateClient(json);
        var result = await client.FetchLatestDieselAsync("NUS");

        Assert.IsNotNull(result);
        Assert.AreEqual("EPD2D", result.ProductCode);
        Assert.AreEqual("No 2 Diesel", result.ProductName);
        Assert.AreEqual("NUS", result.AreaCode);
        Assert.AreEqual("U.S.", result.AreaName);
        Assert.AreEqual(new DateTime(2026, 9, 28), result.Period);
        Assert.AreEqual(3.744m, result.Value);
        Assert.AreEqual("$/GAL", result.Unit);
    }

    /// <summary>
    /// Tests empty EIA response.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetLatestDieselAsync_WithEmptyData_ShouldReturnNull()
    {
        const string json = """
        {
            "response": {
                "data": []
            }
        }
        """;

        var client = CreateClient(json);
        var result = await client.FetchLatestDieselAsync("NUS");

        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests an invalid value.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetLatestDieselAsync_WithInvalidValue_ShouldReturnNull()
    {
        const string json = """
        {
            "response": {
                "data": [
                    {
                        "period": "2026-09-28",
                        "duoarea": "NUS",
                        "area-name": "U.S.",
                        "product": "EPD2D",
                        "product-name": "No 2 Diesel",
                        "value": "not-a-number",
                        "units": "$/GAL"
                    }
                ]
            }
        }
        """;

        var client = CreateClient(json);
        var result = await client.FetchLatestDieselAsync("NUS");

        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests missing Period field.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetLatestDieselAsync_WithMissingPeriod_ShouldReturnNull()
    {
        const string json = """
        {
            "response": {
                "data": [
                    {
                        "duoarea": "NUS",
                        "area-name": "U.S.",
                        "product": "EPD2D",
                        "product-name": "No 2 Diesel",
                        "value": "3.744",
                        "units": "$/GAL"
                    }
                ]
            }
        }
        """;

        var client = CreateClient(json);
        var result = await client.FetchLatestDieselAsync("NUS");

        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests server error 500.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetLatestDieselAsync_WithErrorStatusCode_ShouldReturnNull()
    {
        var client = CreateClient("{}", HttpStatusCode.InternalServerError);
        var result = await client.FetchLatestDieselAsync("NUS");

        Assert.IsNull(result);
    }
}
