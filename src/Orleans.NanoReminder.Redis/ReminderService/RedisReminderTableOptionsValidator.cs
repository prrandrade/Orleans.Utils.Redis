namespace Orleans.NanoReminder.Redis.ReminderService
{
    using Microsoft.Extensions.Options;
    using Runtime;

    public class RedisReminderTableOptionsValidator : IConfigurationValidator
    {
        private readonly RedisReminderTableOptions _options;
        
        public RedisReminderTableOptionsValidator(IOptions<RedisReminderTableOptions> options)
        {
            _options = options.Value ?? throw new OrleansConfigurationException($"{nameof(options)} is required.");
        }
        
        public void ValidateConfiguration()
        {
            if (_options.ConfigurationOptions == null)
                throw new OrleansConfigurationException($"Null value for {nameof(RedisReminderTableOptions.ConfigurationOptions)}.");
        }
    }
}
