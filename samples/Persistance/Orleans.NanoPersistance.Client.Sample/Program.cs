namespace Orleans.NanoPersistance.Client.Sample
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces.Sample;

    internal class Program
    {
        private static async Task<int> Main()
        {
            try
            {
                await using var client = await ConnectClient();

                int rand1, rand2;
                var rand = new Random();

                do
                {
                    rand1 = rand.Next(0, 400);
                    rand2 = rand.Next(0, 400);
                } while (rand1 == rand2);
                
                var grain1 = client.GetGrain<ITestGrain>(rand1);
                var grain2 = client.GetGrain<ITestGrain>(rand2);

                
                Console.WriteLine("Creating infos:");
                await grain1.UpdateInformationAsync("blablabla1");
                Thread.Sleep(500);
                await grain2.UpdateInformationAsync("blablabla2");
                var result1 = await grain1.RetrieveInformationAsync();
                var result2 = await grain2.RetrieveInformationAsync();
                Console.WriteLine($"grain 1: {result1.Information} - {result1.UpdatedDate}");
                Console.WriteLine($"grain 2: {result2.Information} - {result2.UpdatedDate}");
                Console.WriteLine();
                
                Console.WriteLine("Updating infos:");
                await grain1.UpdateInformationAsync("new blablabla1");
                Thread.Sleep(500);
                await grain2.UpdateInformationAsync("new blablabla2");
                result1 = await grain1.RetrieveInformationAsync();
                result2 = await grain2.RetrieveInformationAsync();
                Console.WriteLine($"grain 1: {result1.Information} - {result1.UpdatedDate}");
                Console.WriteLine($"grain 2: {result2.Information} - {result2.UpdatedDate}");
                Console.WriteLine();

                Console.WriteLine("Erasing infos:");
                await grain1.CleanInformationAsync();
                Thread.Sleep(500);
                await grain2.CleanInformationAsync();
                result1 = await grain1.RetrieveInformationAsync();
                result2 = await grain2.RetrieveInformationAsync();
                Console.WriteLine($"grain 1: {result1.Information} - {result1.UpdatedDate}");
                Console.WriteLine($"grain 2: {result2.Information} - {result2.UpdatedDate}");
                Console.WriteLine();
                
                Console.WriteLine("Creating new infos:");
                await grain1.UpdateInformationAsync("blablabla1");
                Thread.Sleep(500);
                await grain2.UpdateInformationAsync("blablabla2");
                result1 = await grain1.RetrieveInformationAsync();
                result2 = await grain2.RetrieveInformationAsync();
                Console.WriteLine($"grain 1: {result1.Information} - {result1.UpdatedDate}");
                Console.WriteLine($"grain 2: {result2.Information} - {result2.UpdatedDate}");
                await grain1.CleanInformationAsync();
                await grain2.CleanInformationAsync();
                Console.WriteLine("Finished process...");
                Console.ReadKey(true);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
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
