using Microsoft.Extensions.Logging;
using Moq;
using Server.Functions;

namespace Server.Tests;

public class KeepAliveFunctionTests
{
    private readonly Mock<ILogger<KeepAliveFunction>> _mockLogger;
    private readonly KeepAliveFunction _function;

    public KeepAliveFunctionTests()
    {
        _mockLogger = new Mock<ILogger<KeepAliveFunction>>();
        _function = new KeepAliveFunction(_mockLogger.Object);
    }

    [Fact]
    public void KeepAliveFunction_CanBeInstantiated()
    {
        // Arrange & Act
        var function = new KeepAliveFunction(_mockLogger.Object);

        // Assert
        Assert.NotNull(function);
    }

    [Fact]
    public void TimerKeepAlive_ExecutesWithoutError()
    {
        // Arrange
        var timerInfo = new MyTimerInfo
        {
            ScheduleStatus = new MyScheduleStatus
            {
                Last = DateTime.UtcNow.AddMinutes(-4),
                Next = DateTime.UtcNow.AddMinutes(4),
                LastUpdated = DateTime.UtcNow
            }
        };

        // Act & Assert - Should not throw
        _function.TimerKeepAlive(timerInfo);

        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Timer keep-alive executed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void TimerKeepAlive_HandlesNullScheduleStatus()
    {
        // Arrange
        var timerInfo = new MyTimerInfo
        {
            ScheduleStatus = null
        };

        // Act & Assert - Should not throw
        _function.TimerKeepAlive(timerInfo);

        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Timer keep-alive executed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void MyTimerInfo_PropertiesWork()
    {
        // Arrange
        var scheduleStatus = new MyScheduleStatus
        {
            Last = DateTime.UtcNow.AddMinutes(-5),
            Next = DateTime.UtcNow.AddMinutes(5),
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var timerInfo = new MyTimerInfo
        {
            ScheduleStatus = scheduleStatus
        };

        // Assert
        Assert.NotNull(timerInfo.ScheduleStatus);
        Assert.Equal(scheduleStatus.Last, timerInfo.ScheduleStatus.Last);
        Assert.Equal(scheduleStatus.Next, timerInfo.ScheduleStatus.Next);
        Assert.Equal(scheduleStatus.LastUpdated, timerInfo.ScheduleStatus.LastUpdated);
    }
}