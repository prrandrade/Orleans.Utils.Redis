namespace Orleans.NanoPersistance.Redis.GrainStorage
{
    using System;
    using Configuration.Overrides;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Storage;

    public static class RedisGrainStorageFactory
    {
        internal static IGrainStorage Create(IServiceProvider services, string name)
        {
            var optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<RedisGrainStorageOptions>>();

            var serviceName = name;
            var options = optionsSnapshot.Get(name);
            var clusterOptions = services.GetProviderClusterOptions(name).Value;

            var storage = ActivatorUtilities.CreateInstance<RedisGrainStorage>(services, serviceName, options, clusterOptions);
            return storage;
        }
    }
}
