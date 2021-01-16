namespace Orleans.NanoPersistance.Redis.GrainStorage
{
    using StackExchange.Redis;

    public class RedisGrainStorageOptions
    {
        public ConfigurationOptions ConfigurationOptions { get; set; }

        public string KeyPrefix { get; set; }
    }
}
