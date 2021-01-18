namespace Orleans.NanoReminder.Redis.ReminderService
{
    using System;

    public class RedisReminderTableData
    {
        public string ServiceId { get; set; }
        
        public string GrainId { get; set; }
        
        public string ReminderName { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public double Period { get; set; }
        
        public uint GrainHash { get; set; }
        
        public uint Version { get; set; }
    }
}
