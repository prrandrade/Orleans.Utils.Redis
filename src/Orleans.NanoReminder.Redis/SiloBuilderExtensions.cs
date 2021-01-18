namespace Orleans.NanoReminder.Redis
{
    using System;
    using Configuration;
    using Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using ReminderService;

    public static class SiloBuilderExtensions
    {
        /// <summary>
        /// Adds a Redis reminder engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloHostBuilder with Redis reminder engine</returns>
        public static ISiloHostBuilder UseRedisReminderService(
            this ISiloHostBuilder builder,
            Action<RedisReminderTableOptions> configureOptions)
        {
            return builder.UseRedisReminderService(ob => ob.Configure(configureOptions));
        }
        
        /// <summary>
        /// Adds a Redis reminder engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloHostBuilder with Redis reminder engine</returns>
        public static ISiloHostBuilder UseRedisReminderService(
            this ISiloHostBuilder builder,
            Action<OptionsBuilder<RedisReminderTableOptions>> configureOptions)
        {
            return builder.ConfigureServices(services => services.UseRedisReminderService(configureOptions));
        }
        
        /// <summary>
        /// Adds a Redis reminder engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloBuilder with Redis reminder engine</returns>
        public static ISiloBuilder UseRedisReminderService(
            this ISiloBuilder builder,
            Action<RedisReminderTableOptions> configureOptions)
        {
            return builder.UseRedisReminderService(ob => ob.Configure(configureOptions));
        }
        
        /// <summary>
        /// Adds a Redis reminder engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloBuilder with Redis reminder engine</returns>
        public static ISiloBuilder UseRedisReminderService(
            this ISiloBuilder builder,
            Action<OptionsBuilder<RedisReminderTableOptions>> configureOptions)
        {
            return builder.ConfigureServices(services => services.UseRedisReminderService(configureOptions));
        }
        
        /// <summary>
        /// Adds a Redis reminder engine
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>IServiceCollection with Redis reminder engine</returns>
        public static IServiceCollection UseRedisReminderService(
            this IServiceCollection services, 
            Action<OptionsBuilder<RedisReminderTableOptions>> configureOptions)
        {
            services.AddSingleton<IReminderTable, RedisReminderTable>();
            services.ConfigureFormatter<RedisReminderTableOptions>();
            services.AddSingleton<IConfigurationValidator, RedisReminderTableOptionsValidator>();
            configureOptions(services.AddOptions<RedisReminderTableOptions>());
            return services;
        }
    }
}
