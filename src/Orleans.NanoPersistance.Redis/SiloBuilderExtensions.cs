namespace Orleans.NanoPersistance.Redis
{
    using System;
    using Configuration;
    using GrainStorage;
    using Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using Providers;
    using Runtime;
    using Storage;

    public static class SiloBuilderExtensions
    {
        /// <summary>
        /// Adds a default Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloHostBuilder with Redis storage engine</returns>
        public static ISiloHostBuilder AddRedisGrainStorageAsDefault(this ISiloHostBuilder builder, Action<RedisGrainStorageOptions> configureOptions)
        {
            return builder.AddRedisGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Adds a named Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">Storage name</param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloHostBuilder with Redis storage engine</returns>
        public static ISiloHostBuilder AddRedisGrainStorage(this ISiloHostBuilder builder, string name, Action<RedisGrainStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddRedisGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Adds a default Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloHostBuilder with Redis storage engine</returns>
        public static ISiloHostBuilder AddRedisGrainStorageAsDefault(this ISiloHostBuilder builder, Action<OptionsBuilder<RedisGrainStorageOptions>> configureOptions = null)
        {
            return builder.AddRedisGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Adds a named Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">Storage name</param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloHostBuilder with Redis storage engine</returns>
        public static ISiloHostBuilder AddRedisGrainStorage(this ISiloHostBuilder builder, string name, Action<OptionsBuilder<RedisGrainStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddRedisGrainStorage(name, configureOptions));
        }
        
        /// <summary>
        /// Adds a default Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloBuilder with Redis storage engine</returns>
        public static ISiloBuilder AddRedisGrainStorageAsDefault(this ISiloBuilder builder, Action<RedisGrainStorageOptions> configureOptions)
        {
            return builder.AddRedisGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Adds a named Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">Storage name</param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloBuilder with Redis storage engine</returns>
        public static ISiloBuilder AddRedisGrainStorage(this ISiloBuilder builder, string name, Action<RedisGrainStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddRedisGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Adds a default Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloBuilder with Redis storage engine</returns>
        public static ISiloBuilder AddRedisGrainStorageAsDefault(this ISiloBuilder builder, Action<OptionsBuilder<RedisGrainStorageOptions>> configureOptions = null)
        {
            return builder.AddRedisGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Adds a named Redis storage engine
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">Storage name</param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>ISiloBuilder with Redis storage engine</returns>
        public static ISiloBuilder AddRedisGrainStorage(this ISiloBuilder builder, string name, Action<OptionsBuilder<RedisGrainStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddRedisGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Adds a default Redis storage engine
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>IServiceCollection with Redis storage engine</returns>
        public static IServiceCollection AddRedisGrainStorage(this IServiceCollection services, Action<RedisGrainStorageOptions> configureOptions)
        {
            return services.AddRedisGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, ob => ob.Configure(configureOptions));
        }

        /// <summary>
        /// Adds a named Redis storage engine
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">Storage name</param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>IServiceCollection with Redis storage engine</returns>
        public static IServiceCollection AddRedisGrainStorage(this IServiceCollection services, string name, Action<RedisGrainStorageOptions> configureOptions)
        {
            return services.AddRedisGrainStorage(name, ob => ob.Configure(configureOptions));
        }

        /// <summary>
        /// Adds a default Redis storage engine
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>IServiceCollection with Redis storage engine</returns>
        public static IServiceCollection AddRedisGrainStorageAsDefault(this IServiceCollection services, Action<OptionsBuilder<RedisGrainStorageOptions>> configureOptions = null)
        {
            return services.AddRedisGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Adds a named Redis storage engine
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">Storage name</param>
        /// <param name="configureOptions">ConfigureOptions</param>
        /// <returns>IServiceCollection with Redis storage engine</returns>
        public static IServiceCollection AddRedisGrainStorage(this IServiceCollection services, string name, Action<OptionsBuilder<RedisGrainStorageOptions>> configureOptions = null)
        {
            services.AddSingleton<IRedisGrainStorageService, RedisGrainStorageService>();
            
            configureOptions?.Invoke(services.AddOptions<RedisGrainStorageOptions>(name));
            
            services.ConfigureNamedOptionForLogging<RedisGrainStorageOptions>(name);
            services.TryAddSingleton(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            services.AddTransient<IConfigurationValidator>(sp => new RedisGrainStorageOptionsValidator(sp.GetRequiredService<IOptionsMonitor<RedisGrainStorageOptions>>().Get(name), name));
            
            return services
                .AddSingletonNamedService<IGrainStorage>(name, RedisGrainStorageFactory.Create)
                .AddSingletonNamedService<ILifecycleParticipant<ISiloLifecycle>>(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }
    }
}