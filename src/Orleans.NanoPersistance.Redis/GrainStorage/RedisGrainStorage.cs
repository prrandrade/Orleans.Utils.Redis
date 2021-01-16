namespace Orleans.NanoPersistance.Redis.GrainStorage
{
    using System.Threading.Tasks;
    using Configuration;
    using Runtime;
    using StackExchange.Redis;
    using Storage;

    public class RedisGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly IRedisGrainStorageService _storageService;
        
        public string StorageName { get; }
        public RedisGrainStorageOptions Options { get; }
        public ClusterOptions ClusterOptions { get; }
        public ITypeResolver TypeResolver { get; }
        public IDatabase Database { get; private set; }

        public RedisGrainStorage(
            IRedisGrainStorageService storageService,
            string storageName,
            RedisGrainStorageOptions options,
            ClusterOptions clusterOptions,
            ITypeResolver typeResolver)
        {
            _storageService = storageService;
            StorageName = storageName;
            Options = options;
            ClusterOptions = clusterOptions;
            TypeResolver = typeResolver;
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<RedisGrainStorage>(StorageName), ServiceLifecycleStage.ApplicationServices,
                ct =>
                {
                    var result = ConnectionMultiplexer.Connect(Options.ConfigurationOptions);
                    Database = result.GetDatabase();
                    return Task.CompletedTask;
                });
        }

        public string GetKeyString(string grainType, GrainReference grainReference)
        {
            return $"{Options.KeyPrefix}:{ClusterOptions.ServiceId}:{grainReference.ToKeyString()}:{grainType}";
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var key = GetKeyString(grainType, grainReference);
            await _storageService.ReadStateAsync(Database, TypeResolver, grainState, key);
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var key = GetKeyString(grainType, grainReference);
            await _storageService.WriteStateAsync(Database, grainState, key);
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var key = GetKeyString(grainType, grainReference);
            await _storageService.ClearStateAsync(Database, key);
        }
    }
}
