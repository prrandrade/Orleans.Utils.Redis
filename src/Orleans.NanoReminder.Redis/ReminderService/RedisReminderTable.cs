namespace Orleans.NanoReminder.Redis.ReminderService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Options;
    using Runtime;
    using StackExchange.Redis;

    public class RedisReminderTable : IReminderTable
    {
        public IDatabase Database { get; set; }
        private readonly IGrainReferenceConverter _converter;
        private readonly RedisReminderTableOptions _options;
        private readonly string _serviceId;

        public RedisReminderTable(
            IGrainReferenceConverter converter,
            IOptions<ClusterOptions> clusterOptions,
            IOptions<RedisReminderTableOptions> options)
        {
            _converter = converter;
            _options = options.Value;
            _serviceId = clusterOptions.Value.ServiceId;
        }

        public string GetKeyStringForGrainServiceId()
        {
            return $"{_options.KeyPrefix}.{_serviceId}";
        }

        public string GetKeyStringForGrainReferences(string grainId)
        {
            return $"{_options.KeyPrefix}.{_serviceId}.{grainId}";
        }

        public string GetKeyStringForReminder(string grainId, string reminderName)
        {
            return $"{_options.KeyPrefix}.{_serviceId}.{grainId}.{reminderName}";
        }

        public string GetKeyStringForHash()
        {
            return $"{_options.KeyPrefix}.{_serviceId}.grainhash";
        }

        public async Task Init()
        {
            var connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions);
            Database = connection.GetDatabase();
            if (!await Database.KeyExistsAsync(GetKeyStringForHash()))
            {
                var dic = new Dictionary<string, uint>();
                await Database.StringSetAsync(GetKeyStringForHash(), JsonSerializer.Serialize(dic));
            }
        }

        public async Task<ReminderTableData> ReadRows(GrainReference grainRef)
        {
            var keys = await GetAllKeysFromGrainReferenceAsync(grainRef.ToKeyString());

            var values = (await Database.StringGetAsync(keys))
                .Select(x => JsonSerializer.Deserialize<RedisReminderTableData>(x))
                .Select(y => new ReminderEntry
                {
                    GrainRef = _converter.GetGrainFromKeyString(y.GrainId),
                    Period = TimeSpan.FromMilliseconds(y.Period),
                    ReminderName = y.ReminderName,
                    ETag = y.Version.ToString(),
                    StartAt = y.StartTime,
                });

            return new ReminderTableData(values);
        }

        public async Task<ReminderTableData> ReadRows(uint beginHash, uint endHash)
        {
            
            var dic = JsonSerializer.Deserialize<Dictionary<string, uint>>(await Database.StringGetAsync(GetKeyStringForHash()));
            RedisKey[] keys;

            if (beginHash < endHash)
            {
                keys = dic
                    .Where(x => x.Value > beginHash && x.Value <= endHash)
                    .Select(y => new RedisKey(y.Key))
                    .ToArray();
            }
            else
            {
                keys = dic
                    .Where(x => x.Value > beginHash || x.Value <= endHash)
                    .Select(y => new RedisKey(y.Key))
                    .ToArray();
            }

            var values = await Database.StringGetAsync(keys);
            var tableData = values.Select(x => JsonSerializer.Deserialize<RedisReminderTableData>(x)).ToList();
            var reminders = tableData
                .Select(y => new ReminderEntry
                {
                    Period = TimeSpan.FromMilliseconds(y.Period),
                    ReminderName = y.ReminderName,
                    ETag = y.Version.ToString(),
                    StartAt = y.StartTime,
                    GrainRef = _converter.GetGrainFromKeyString(y.GrainId)
                }).ToList();

            var reminderTableData = new ReminderTableData(reminders);
            
            if (reminders.Count > 0)
                Console.WriteLine("OPA!");
            
            return reminderTableData;
        }

        public async Task<ReminderEntry> ReadRow(GrainReference grainRef, string reminderName)
        {
            var key = GetKeyStringForReminder(grainRef.ToKeyString(), reminderName);
            var tableData = JsonSerializer.Deserialize<RedisReminderTableData>(await Database.StringGetAsync(key));
            return new ReminderEntry
            {
                Period = TimeSpan.FromMilliseconds(tableData.Period),
                ReminderName = tableData.ReminderName,
                ETag = tableData.Version.ToString(),
                StartAt = tableData.StartTime,
                GrainRef = _converter.GetGrainFromKeyString(tableData.GrainId)
            };
        }

        public async Task<string> UpsertRow(ReminderEntry entry)
        {
            string returnData;

            var key = GetKeyStringForReminder(entry.GrainRef.ToKeyString(), entry.ReminderName);
            await InsertIndividualKeyForServiceId(key);
            await InsertInvidualKeyForGrainReferenceAsync(entry.GrainRef.ToKeyString(), key);

            if (await Database.KeyExistsAsync(key))
            {
                var oldData = JsonSerializer.Deserialize<RedisReminderTableData>(await Database.StringGetAsync(key));
                var newData = new RedisReminderTableData
                {
                    ServiceId = _serviceId,
                    GrainId = entry.GrainRef.ToKeyString(),
                    ReminderName = entry.ReminderName,
                    StartTime = entry.StartAt,
                    Period = entry.Period.TotalMilliseconds,
                    GrainHash = entry.GrainRef.GetUniformHashCode(),
                    Version = oldData.Version + 1
                };
                var stringData = JsonSerializer.Serialize(newData);
                await Database.StringSetAsync(key, stringData);
                returnData = newData.Version.ToString();
            }
            else
            {
                var data = new RedisReminderTableData
                {
                    ServiceId = _serviceId,
                    GrainId = entry.GrainRef.ToKeyString(),
                    ReminderName = entry.ReminderName,
                    StartTime = entry.StartAt,
                    Period = entry.Period.TotalMilliseconds,
                    GrainHash = entry.GrainRef.GetUniformHashCode(),
                    Version = 0
                };
                var stringData = JsonSerializer.Serialize(data);
                await Database.StringSetAsync(key, stringData);
                returnData = data.Version.ToString();
            }

            await UpsertIndividualKeyForHash(key, entry.GrainRef.GetUniformHashCode());

            return returnData;
        }

        public async Task<bool> RemoveRow(GrainReference grainRef, string reminderName, string eTag)
        {
            bool returnData;
            var key = GetKeyStringForReminder(grainRef.ToKeyString(), reminderName);

            if (await Database.KeyExistsAsync(key))
            {
                var data = JsonSerializer.Deserialize<RedisReminderTableData>(await Database.StringGetAsync(key));
                if (data.Version.ToString() == eTag)
                {
                    await Database.KeyDeleteAsync(key);
                    returnData = true;
                }
                else
                    returnData = false;
            }
            else
                returnData = false;

            if (returnData)
            {
                await DeleteIndividualKeyForHash(key);
                await DeleteIndividualKeyForServiceIdAsync(key);
                await DeleteIndividualKeyForGrainReferenceAsync(grainRef.ToKeyString(), key);
            }

            return returnData;
        }

        public async Task TestOnlyClearTable()
        {
            var keys = await GetAllKeysFromServiceIdAsync();
            await Database.KeyDeleteAsync(keys);
            await DeleteIndividualKeyForHash(keys);
        }



        // INTERNAL METHODS

        internal async Task UpsertIndividualKeyForHash(string key, uint grainHash)
        {
            var dic = JsonSerializer.Deserialize<Dictionary<string, uint>>(await Database.StringGetAsync(GetKeyStringForHash()));
            dic[key] = grainHash;
            await Database.StringSetAsync(GetKeyStringForHash(), JsonSerializer.Serialize(dic));
        }

        internal async Task DeleteIndividualKeyForHash(params RedisKey[] keys)
        {
            var dic = JsonSerializer.Deserialize<Dictionary<string, uint>>(await Database.StringGetAsync(GetKeyStringForHash()));
            foreach (var key in keys)
                dic.Remove(key);

            await Database.StringSetAsync(GetKeyStringForHash(), JsonSerializer.Serialize(dic));
        }

        internal async Task InsertIndividualKeyForServiceId(string key)
        {
            var keyServiceId = GetKeyStringForGrainServiceId();
            if (!Database.KeyExistsAsync(keyServiceId).Result)
            {
                var value = JsonSerializer.Serialize(new List<string> { key });
                await Database.StringSetAsync(keyServiceId, value);
            }
            else
            {
                var listOfKeysForThisGrainRef = Database.StringGetAsync(keyServiceId).Result;
                var result = JsonSerializer.Deserialize<List<string>>(listOfKeysForThisGrainRef);
                if (!result.Contains(key))
                    result.Add(key);
                await Database.StringSetAsync(keyServiceId, JsonSerializer.Serialize(result));
            }
        }

        internal async Task<RedisKey[]> GetAllKeysFromServiceIdAsync()
        {
            var keyServiceId = GetKeyStringForGrainServiceId();
            var keys = JsonSerializer.Deserialize<List<string>>(await Database.StringGetAsync(keyServiceId));
            return keys.Select(x => new RedisKey(x)).ToArray();
        }

        internal async Task DeleteIndividualKeyForServiceIdAsync(string key)
        {
            var keyServiceId = GetKeyStringForGrainServiceId();

            if (await Database.KeyExistsAsync(keyServiceId))
            {
                var listOfKeysForThisGrainRef = await Database.StringGetAsync(keyServiceId);
                var result = JsonSerializer.Deserialize<List<string>>(listOfKeysForThisGrainRef);
                result.Remove(key);
                await Database.StringSetAsync(keyServiceId, JsonSerializer.Serialize(result));
            }
        }

        internal async Task InsertInvidualKeyForGrainReferenceAsync(string grainRefKeyString, string key)
        {
            var keyGrainRef = GetKeyStringForGrainReferences(grainRefKeyString);
            if (!await Database.KeyExistsAsync(keyGrainRef))
            {
                await Database.StringSetAsync(keyGrainRef, JsonSerializer.Serialize(new List<string> { key }));
            }
            else
            {
                var listOfKeysForThisGrainRef = await Database.StringGetAsync(keyGrainRef);
                var result = JsonSerializer.Deserialize<List<string>>(listOfKeysForThisGrainRef);
                if (!result.Contains(key))
                    result.Add(key);
                await Database.StringSetAsync(keyGrainRef, JsonSerializer.Serialize(result));
            }
        }

        internal async Task<RedisKey[]> GetAllKeysFromGrainReferenceAsync(string grainRefKeyString)
        {
            var keyGranRefId = GetKeyStringForGrainReferences(grainRefKeyString);
            var keys = JsonSerializer.Deserialize<List<string>>(await Database.StringGetAsync(keyGranRefId));
            return keys.Select(x => new RedisKey(x)).ToArray();
        }

        internal async Task DeleteIndividualKeyForGrainReferenceAsync(string grainRefKeyString, string key)
        {
            var keyGrainRef = GetKeyStringForGrainReferences(grainRefKeyString);

            if (await Database.KeyExistsAsync(keyGrainRef))
            {
                var listOfKeysForThisGrainRef = await Database.StringGetAsync(keyGrainRef);
                var result = JsonSerializer.Deserialize<List<string>>(listOfKeysForThisGrainRef);
                result.Remove(key);
                await Database.StringSetAsync(keyGrainRef, JsonSerializer.Serialize(result));
            }
        }
    }
}
