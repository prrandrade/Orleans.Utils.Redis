namespace Orleans.NanoPersistance.Redis.GrainStorage
{
    using Runtime;

    public class RedisGrainStorageOptionsValidator : IConfigurationValidator
    {
        private readonly RedisGrainStorageOptions _options;

        public RedisGrainStorageOptionsValidator(RedisGrainStorageOptions configurationOptions, string name)
        {
            _options = configurationOptions ?? throw new OrleansConfigurationException("Options is required.");
        }

        public void ValidateConfiguration()
        {
            if (_options.ConfigurationOptions == null)
                throw new OrleansConfigurationException($"Null value for {nameof(RedisGrainStorageOptions.ConfigurationOptions)}.");
        }
    }
}
