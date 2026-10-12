using FuelPricePipeline.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FuelPricePipeline.UseCase.GetLatest;
using Moq;

namespace FuelPricePipeline.Tests.UseCase;

[TestClass]
public class ExecutorTests
{
    private Mock<IEiaClient> _mockClient = null!;
    private Mock<IFuelRepository> _mockRepo = null!;
    private Mock<ICsvWriter> _mockCsvWriter = null!;
    private Mock<Microsoft.Extensions.Logging.ILogger<Executor>> _mockLogger = null!;
    private Executor _executor = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockClient = new Mock<IEiaClient>();
        _mockRepo = new Mock<IFuelRepository>();
        _mockCsvWriter = new Mock<ICsvWriter>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<Executor>>();
        _executor = new Executor(_mockClient.Object, _mockRepo.Object, _mockCsvWriter.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenClientReturnsData_ShouldReturnSuccess()
    {
        // Arrange
        var expectedPrice = new DieselFuelPrice
        {
            ProductCode = "EPD2D",
            ProductName = "No 2 Diesel",
            AreaCode = "NUS",
            AreaName = "U.S.",
            Period = new DateTime(2025, 8, 1),
            Value = 3.744m,
            Unit = "$/GAL"
        };

        _mockClient.Setup(c => c.FetchLatestDieselAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedPrice);

        // Act
        var result = await _executor.ExecuteAsync("test.csv", "NUS");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("EPD2D", result.Data.ProductCode);
        Assert.AreEqual(3.744m, result.Data.Value);

        _mockRepo.Verify(r => r.UpsertAsync(It.IsAny<DieselFuelPrice>()), Times.Once);
        _mockCsvWriter.Verify(w => w.WriteAsync("test.csv", expectedPrice), Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenClientReturnsNull_ShouldReturnFailure()
    {
        // Arrange
        _mockClient.Setup(c => c.FetchLatestDieselAsync(It.IsAny<string>()))
            .ReturnsAsync((DieselFuelPrice?)null);

        // Act
        var result = await _executor.ExecuteAsync("test.csv", "NUS");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Failed to get data"));

        _mockRepo.Verify(r => r.UpsertAsync(It.IsAny<DieselFuelPrice>()), Times.Never);
        _mockCsvWriter.Verify(w => w.WriteAsync(It.IsAny<string>(), It.IsAny<DieselFuelPrice>()), Times.Never);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        _mockClient.Setup(c => c.FetchLatestDieselAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var result = await _executor.ExecuteAsync("test.csv", "NUS");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Unexpected error"));
    }
}