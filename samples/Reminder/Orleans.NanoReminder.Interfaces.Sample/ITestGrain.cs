namespace Orleans.NanoReminder.Interfaces.Sample
{
    using System.Threading.Tasks;

    public interface ITestGrain : IGrainWithIntegerKey, IRemindable
    {
        Task ActivateReminder();

        Task DeactivateGrain();

        Task DeactivateReminder();
    }
}
