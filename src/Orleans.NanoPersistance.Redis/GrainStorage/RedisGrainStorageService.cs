namespace Orleans.NanoPersistance.Redis.GrainStorage
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Runtime;
    using StackExchange.Redis;

    public interface IRedisGrainStorageService
    {
        Task ReadStateAsync(IDatabase database, ITypeResolver typeResolver, IGrainState grainState, string key);

        Task WriteStateAsync(IDatabase database, IGrainState grainState, string key);

        Task ClearStateAsync(IDatabase database, string key);
    }

    public class RedisGrainStorageService : IRedisGrainStorageService
    {
        public async Task ReadStateAsync(IDatabase database, ITypeResolver typeResolver, IGrainState grainState, string key)
        {
            var storedData = await database.StringGetAsync(key);
            
            if (storedData == RedisValue.Null)
            {
                grainState.State = Activator.CreateInstance(grainState.State.GetType());
                return;
            }
            
            var result = Serialization.Deserialize(storedData, typeResolver);
            grainState.State = result;
            grainState.ETag = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }

        public async Task WriteStateAsync(IDatabase database, IGrainState grainState, string key)
        {
            var storedData = Serialization.Serialize(grainState.State);
            await database.StringSetAsync(key, storedData);
            grainState.ETag = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }

        public async Task ClearStateAsync(IDatabase database, string key)
        {
            await database.KeyDeleteAsync(key);
        }
    }
}
