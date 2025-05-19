using CoreKit.EventHandler.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreKit.EventHandler.Test
{
    public class EventHandlerTests
    {
        [Fact]
        public void PublishEvent_Calls_NotifyListeners_Once()
        {
            // Arrange
            var listenerFactoryMock = new Mock<IEventListenerFactory>();
            var loggerMock = new Mock<ILogger<EventHandler>>();
            var eventHandler = new EventHandler(listenerFactoryMock.Object, loggerMock.Object);
            dynamic payload = new { Message = "Test Message" };
            string process = "TestProcess";
            string action = "TestAction";

            // Act
            eventHandler.PublishEvent(payload, process, action);

            // Assert
            listenerFactoryMock.Verify(lf => lf.GetListeners(process, action), Times.Once);
        }
    }
}
