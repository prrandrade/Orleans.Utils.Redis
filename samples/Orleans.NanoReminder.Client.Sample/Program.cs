namespace Orleans.NanoReminder.Client.Sample
{
    using System;
    using System.Threading.Tasks;
    using Interfaces.Sample;

    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            
            
            await using var client = await ConnectClient();

            var grain = client.GetGrain<ITestGrain>(1);

            
            Console.WriteLine("Pressione uma tecla para ativar o reminder...");
            Console.ReadKey(true);
            await grain.ActivateReminder();
            
            Console.WriteLine("Pressione uma tecla para desativar o grain...");
            Console.ReadKey(true);
            await grain.DeactivateGrain();
            
            Console.WriteLine("Pressione uma tecla para desativar o reminder...");
            Console.ReadKey(true);
            await grain.DeactivateReminder();

            return 0;
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }
    }
}
