using RabbitMQ.Client;

namespace Fluff.Test.Options
{
    [TestClass]
    public class FluffOptionsTest
    {
        [TestMethod]
        public void FluffOptions_HasDefaultValues()
        {
            // Act
            var result = new FluffOptions();

            // Assert
            Assert.AreEqual(FluffOptionsDefaults.HostName, result.HostName);
            Assert.AreEqual(FluffOptionsDefaults.Port, result.Port);
            Assert.AreEqual(FluffOptionsDefaults.UserName, result.UserName);
            Assert.AreEqual(FluffOptionsDefaults.Password, result.Password);
            Assert.AreEqual(FluffOptionsDefaults.ExchangeName, result.ExchangeName);
            Assert.AreEqual(FluffOptionsDefaults.ExchangeType, result.ExchangeType);
            Assert.AreEqual(FluffOptionsDefaults.ExchangeIsDurable, result.ExchangeIsDurable);
            Assert.AreEqual(FluffOptionsDefaults.QueueName, result.QueueName);
        }

        [TestMethod]
        public void CreateFactory_FluffOptions_ReturnsFactory()
        {
            // Arrange
            var sut = new FluffOptions()
            {
                ExchangeName = "Ex",
                QueueName = "Fluff.Test.Event"
            };

            // Act
            var result = sut.CreateFactory();

            // Assert
            Assert.IsInstanceOfType(result, typeof(IConnectionFactory));

            ConnectionFactory factory = (ConnectionFactory)result;
            Assert.AreEqual(sut.HostName, factory.HostName);
            Assert.AreEqual(sut.Port, factory.Port);
            Assert.AreEqual(sut.UserName, factory.UserName);
            Assert.AreEqual(sut.Password, factory.Password);
        }
    }
}
