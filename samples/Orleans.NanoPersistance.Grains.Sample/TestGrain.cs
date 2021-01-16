namespace Orleans.NanoPersistance.Grains.Sample
{
    using System;
    using System.Threading.Tasks;
    using Interfaces.Sample;
    using Models.Sample;
    using Runtime;

    public class TestGrain : Grain, ITestGrain
    {
        private readonly IPersistentState<Model> _state;

        public TestGrain([PersistentState("state", "test")] IPersistentState<Model> state)
        {
            _state = state;
        }

        public async Task UpdateInformationAsync(string information)
        {
            DeactivateOnIdle();

            var obj = new Model
            {
                Information = information,
                UpdatedDate = DateTime.Now
            };

            _state.State = obj;
            await _state.WriteStateAsync();
        }

        public async Task<Model> RetrieveInformationAsync()
        {
            DeactivateOnIdle();
            await _state.ReadStateAsync();
            return _state.State;
        }

        public async Task CleanInformationAsync()
        {
            DeactivateOnIdle();
            await _state.ClearStateAsync();
        }
    }
}
