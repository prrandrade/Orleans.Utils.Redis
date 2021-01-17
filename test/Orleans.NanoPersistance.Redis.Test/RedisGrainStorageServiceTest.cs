namespace Orleans.NanoPersistance.Redis.Test
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using GrainStorage;
    using Moq;
    using Runtime;
    using StackExchange.Redis;
    using Xunit;

    public class ExampleModel
    {
        public int Property1 { get; set; }

        public string Property2 { get; set; }

        public override int GetHashCode()
        {
           return Property1.GetHashCode() + Property2.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(ExampleModel))
                return false;

            return ((ExampleModel) obj).Property1 == Property1 &&
                   ((ExampleModel) obj).Property2 == Property2;
        }
    }
    public class StubGranState : IGrainState {
        public object State { get; set; }
        public Type Type { get; set; }
        public string ETag { get; set; }
        public bool RecordExists { get; set; }
    }

    public class RedisGrainStorageServiceTest
    {
        private readonly RedisGrainStorageService _storageService;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly Mock<ITypeResolver> _typeResolverMock;
        private readonly StubGranState _grainState;

        private readonly ExampleModel _model;

        public RedisGrainStorageServiceTest()
        {
            _storageService = new RedisGrainStorageService();

            _databaseMock = new Mock<IDatabase>();
            _typeResolverMock = new Mock<ITypeResolver>();
            _grainState = new StubGranState();

            _model = new ExampleModel {Property1 = 1, Property2 = "test"};
        }

        [Fact]
        public async Task ReadStateAsync()
        {
            // arrange
            const string key = "key";
            var result = new MessagePackage
            {
                Type = _model.GetType().FullName,
                Payload = JsonSerializer.Serialize(_model)
            };
            
            _databaseMock
                .Setup(x => x.StringGetAsync(key, CommandFlags.None))
                .Returns(Task.FromResult(new RedisValue(JsonSerializer.Serialize(result))));

            _typeResolverMock
                .Setup(x => x.ResolveType(result.Type))
                .Returns(typeof(ExampleModel));

            // act
            await _storageService.ReadStateAsync(
                _databaseMock.Object,
                _typeResolverMock.Object,
                _grainState,
                key);

            // assert
            Assert.Equal(_model, _grainState.State);
        }

        [Fact]
        public async Task ReadStateAsync_NoPreviousObject()
        {
            // arrange
            const string key = "key";
            _grainState.State = new ExampleModel();

            _databaseMock
                .Setup(x => x.StringGetAsync(key, CommandFlags.None))
                .Returns(Task.FromResult(new RedisValue()));

            // act
            await _storageService.ReadStateAsync(
                _databaseMock.Object,
                _typeResolverMock.Object,
                _grainState,
                key);

            // assert
            Assert.Equal(0, ((ExampleModel)_grainState.State).Property1);
            Assert.Null(((ExampleModel)_grainState.State).Property2);
        }

        [Fact]
        public async Task WriteStateAsync()
        {
            // arrange
            const string key = "key";
            _grainState.State = _model;
            
            var result = new MessagePackage
            {
                Type = _model.GetType().FullName,
                Payload = JsonSerializer.Serialize(_model)
            };

            var pack = JsonSerializer.Serialize(result);
            
            // act
            await _storageService.WriteStateAsync(
                _databaseMock.Object,
                _grainState,
                key);

            // assert
            _databaseMock.Verify(x => x.StringSetAsync(key, pack, null, When.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task ClearStateAsync()
        {
            // arrange
            const string key = "key";
            
            // act
            await _storageService.ClearStateAsync(_databaseMock.Object, key);

            // assert
            _databaseMock.Verify(x => x.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
        }
        
    }
}
