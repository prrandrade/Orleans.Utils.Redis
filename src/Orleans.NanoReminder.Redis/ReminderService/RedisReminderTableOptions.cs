namespace Orleans.NanoReminder.Redis.ReminderService
{
    using StackExchange.Redis;

    public class RedisReminderTableOptions
    {
        public ConfigurationOptions ConfigurationOptions { get; set; }

        public string KeyPrefix { get; set; }
    }
}
