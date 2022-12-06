using Microsoft.Extensions.DependencyInjection;

namespace Fluff.Test.Extensions
{
    [TestClass]
    public class FluffExtensionsTest
    {
        private ServiceCollection _services;

        [TestInitialize]
        public void TestInitialize()
        {
            _services = new ServiceCollection();
        }

        [TestMethod]
        public void AddFluff_Options_AddsServices()
        {
            // Arrange
            var options = new FluffOptions();

            // Act
            var result = FluffExtensions.AddFluff(_services, options);

            // Assert
            AssertServices(result);

            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IFluffOptions) &&
                                             s.ImplementationInstance == options &&
                                             s.Lifetime == ServiceLifetime.Singleton));
        }

        public void AddFluff_AddsServices()
        {
            // Act
            var result = FluffExtensions.AddFluff(_services);

            // Assert
            AssertServices(result);
        }

        private void AssertServices(IServiceCollection result)
        {
            Assert.AreEqual(_services, result);

            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IFluffOptions) &&
                                             s.Lifetime == ServiceLifetime.Singleton));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IFluffContext) &&
                                             s.ImplementationType == typeof(FluffContext) &&
                                             s.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IBasicSender) &&
                                             s.ImplementationType == typeof(BasicSender) &&
                                             s.Lifetime == ServiceLifetime.Transient));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IBasicReceiver) &&
                                             s.ImplementationType == typeof(BasicReceiver) &&
                                             s.Lifetime == ServiceLifetime.Transient));

            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FluffHostedService) &&
                                             s.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IFluffPublisher) &&
                                             s.ImplementationType == typeof(FluffPublisher) &&
                                             s.Lifetime == ServiceLifetime.Transient));

            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(ITypeFinder) &&
                                             s.ImplementationType == typeof(TypeFinder) &&
                                             s.Lifetime == ServiceLifetime.Transient));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IRuntimeBuilder) &&
                                             s.ImplementationType == typeof(RuntimeBuilder) &&
                                             s.Lifetime == ServiceLifetime.Transient));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IFluffRouter) &&
                                             s.Lifetime == ServiceLifetime.Transient));
        }

        public void AddFluff_NoEnvironmentVariables_AddsOptionsWithDefaultValues()
        {
            // Act
            FluffExtensions.AddFluff(_services);

            // Assert
            IFluffOptions options = _services.BuildServiceProvider().GetRequiredService<IFluffOptions>();
            Assert.AreEqual(FluffOptionsDefaults.HostName, options.HostName);
            Assert.AreEqual(FluffOptionsDefaults.Port, options.Port);
            Assert.AreEqual(FluffOptionsDefaults.UserName, options.UserName);
            Assert.AreEqual(FluffOptionsDefaults.Password, options.Password);
            Assert.AreEqual(FluffOptionsDefaults.ExchangeName, options.ExchangeName);
            Assert.AreEqual(FluffOptionsDefaults.ExchangeType, options.ExchangeType);
            Assert.AreEqual(FluffOptionsDefaults.ExchangeIsDurable, options.ExchangeIsDurable);
            Assert.AreEqual(FluffOptionsDefaults.QueueName, options.QueueName);
        }

        public void AddFluff_EnvironmentVariables_AddsOptionsWithEnvironmentVariables()
        {
            // Arrange
            string hostName = "external";
            int port = 80;
            string userName = "admin";
            string password = "admin";
            string exchangeName = "Test.Exchange";
            string exchangeType = RabbitMQ.Client.ExchangeType.Fanout;
            bool exchangeIsDurable = false;
            string queueName = "Test.Queue";

            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.HostName, hostName);
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.Port, port.ToString());
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.UserName, userName);
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.Password, password);
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.ExchangeName, exchangeName);
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.ExchangeType, exchangeType);
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.ExchangeIsDurable, exchangeIsDurable.ToString());
            Environment.SetEnvironmentVariable(EnvironmentVariablesNames.QueueName, queueName);

            // Act
            FluffExtensions.AddFluff(_services);

            // Assert
            IFluffOptions options = _services.BuildServiceProvider().GetRequiredService<IFluffOptions>();
            Assert.AreEqual(hostName, options.HostName);
            Assert.AreEqual(port, options.Port);
            Assert.AreEqual(userName, options.UserName);
            Assert.AreEqual(password, options.Password);
            Assert.AreEqual(exchangeName, options.ExchangeName);
            Assert.AreEqual(exchangeType, options.ExchangeType);
            Assert.AreEqual(exchangeIsDurable, options.ExchangeIsDurable);
            Assert.AreEqual(queueName, options.QueueName);
        }
    }
}
