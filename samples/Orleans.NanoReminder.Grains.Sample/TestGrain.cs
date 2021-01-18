namespace Orleans.NanoReminder.Grains.Sample
{
    using System;
    using System.Threading.Tasks;
    using Interfaces.Sample;
    using Microsoft.Extensions.Logging;
    using Runtime;

    public class TestGrain : Grain, ITestGrain
    {
        private readonly ILogger<TestGrain> _logger;
        private int _value;

        public TestGrain(ILogger<TestGrain> logger)
        {
            _logger = logger;
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName == "reminder1")
            {
                await Task.Factory.StartNew(() =>
                {
                    _value++;
                    _logger.LogInformation($"{status.CurrentTickTime:HH:mm:ss.ffff} - New value is {_value}");
                });
            }
        }

        public async Task ActivateReminder()
        {
            await RegisterOrUpdateReminder("reminder1", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60));
        }

        public async Task DeactivateGrain()
        {
            DeactivateOnIdle();
            await Task.CompletedTask;
        }

        public async Task DeactivateReminder()
        {
            var reminder = await GetReminder("reminder1");
            if (reminder != null)
                await UnregisterReminder(reminder);
        }
    }
}
