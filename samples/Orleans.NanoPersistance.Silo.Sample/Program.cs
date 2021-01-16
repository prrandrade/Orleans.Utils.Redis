namespace Orleans.NanoPersistance.Silo.Sample
{
    using System;
    using System.Threading.Tasks;
    using Grains.Sample;
    using Hosting;
    using Microsoft.Extensions.Logging;
    using Redis;
    using StackExchange.Redis;

    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();
                await host.StopAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                
                // named storage engine
                .AddRedisGrainStorage("test", options =>
                {
                    options.ConfigurationOptions = ConfigurationOptions.Parse("127.0.0.1:6379,allowAdmin=true");
                    options.KeyPrefix = "orleans.persistance";
                })
                
                // default storage engine, name is not necessry for objects
                .AddRedisGrainStorageAsDefault(options =>
                {
                    options.ConfigurationOptions = ConfigurationOptions.Parse("127.0.0.1:6379,allowAdmin=true");
                    options.KeyPrefix = "orleans.persistance";
                })

                .UseLocalhostClustering()
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(TestGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
