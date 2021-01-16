namespace Orleans.NanoPersistance.Interfaces.Sample
{
    using System.Threading.Tasks;
    using Models.Sample;

    public interface ITestGrain : IGrainWithIntegerKey
    {
        public Task UpdateInformationAsync(string information);

        public Task<Model> RetrieveInformationAsync();

        public Task CleanInformationAsync();
    }
}
