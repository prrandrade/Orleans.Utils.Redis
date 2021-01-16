namespace Orleans.NanoPersistance.Redis.GrainStorage
{
    using System.Text.Json;
    using Runtime;

    internal static class Serialization
    {
        public static string Serialize(object message)
        {
            var payload = JsonSerializer.Serialize(message);
            var package = new MessagePackage
            {
                Type = message.GetType().FullName,
                Payload = payload
            };
            return JsonSerializer.Serialize(package);
        }

        public static object Deserialize(string data, ITypeResolver typeResolver)
        {
            var package = JsonSerializer.Deserialize<MessagePackage>(data);
            var type = typeResolver.ResolveType(package.Type);
            return JsonSerializer.Deserialize(package.Payload, type);
        }
    }

    internal class MessagePackage
    {
        public string Type { get; set; }

        public string Payload { get; set; }
    }
}
