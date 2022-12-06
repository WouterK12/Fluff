using Microsoft.Extensions.DependencyInjection;

namespace Fluff
{
    public static class FluffExtensions
    {
        public static IServiceCollection AddFluff(this IServiceCollection services, IFluffOptions fluffOptions)
        {
            services.AddSingleton<IFluffOptions>(fluffOptions);
            services.AddSingleton<IFluffContext, FluffContext>();

            services.AddTransient<IBasicSender, BasicSender>();
            services.AddTransient<IBasicReceiver, BasicReceiver>();

            services.AddHostedService<FluffHostedService>();

            services.AddTransient<IFluffPublisher, FluffPublisher>();

            services.AddTransient<ITypeFinder, TypeFinder>();
            services.AddTransient<IRuntimeBuilder, RuntimeBuilder>();
            services.AddTransient<IFluffRouter>(sp => sp.GetRequiredService<IRuntimeBuilder>()
                                                        .DiscoverAndRegisterAllEventListeners()
                                                        .Build());

            return services;
        }

        /// <summary>
        /// Add Fluff with Environment Variables as options
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFluff(this IServiceCollection services)
        {
            IFluffOptions options = new FluffOptions()
            {
                HostName = Environment.GetEnvironmentVariable(EnvironmentVariablesNames.HostName) ?? FluffOptionsDefaults.HostName,
                Port = int.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariablesNames.Port), out int port) ? port : FluffOptionsDefaults.Port,
                UserName = Environment.GetEnvironmentVariable(EnvironmentVariablesNames.UserName) ?? FluffOptionsDefaults.UserName,
                Password = Environment.GetEnvironmentVariable(EnvironmentVariablesNames.Password) ?? FluffOptionsDefaults.Password,
                ExchangeName = Environment.GetEnvironmentVariable(EnvironmentVariablesNames.ExchangeName) ?? FluffOptionsDefaults.ExchangeName,
                ExchangeType = Environment.GetEnvironmentVariable(EnvironmentVariablesNames.ExchangeType) ?? FluffOptionsDefaults.ExchangeType,
                ExchangeIsDurable = bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariablesNames.ExchangeIsDurable), out bool isDurable) ? isDurable : FluffOptionsDefaults.ExchangeIsDurable,
                QueueName = Environment.GetEnvironmentVariable(EnvironmentVariablesNames.QueueName) ?? FluffOptionsDefaults.QueueName,
            };

            return AddFluff(services, options);
        }
    }
}
