using Database.Interface;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Threading.Tasks;

namespace Database.Redis
{
    public class RedisCache: IRedisCache, IDisposable
    {

        private readonly IRedisClient _redisClient;
        private readonly IRedisConnectionPoolManager _redisConnectionPoolManager;

        private readonly RedisConfiguration _redisConfiguration;

        private static readonly Lazy<RedisConfiguration> RedisConfiguration = new Lazy<RedisConfiguration>(() =>
        {
            var redisConfigOptions = new RedisConfiguration
            {
                AbortOnConnectFail = true,
                KeyPrefix = "MyPrefix_",
                Hosts = new[]
                {
                    new RedisHost { Host = "localhost", Port = 6379 }
                },
                AllowAdmin = true,
                ConnectTimeout = 3000,
                Database = 0,
                PoolSize = 5,
                ServerEnumerationStrategy = new ServerEnumerationStrategy()
                {
                    Mode = ServerEnumerationStrategy.ModeOptions.All,
                    TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                    UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
                }
            };

            return redisConfigOptions;
        });

        public RedisCache()
        {
            try
            {
                _redisConfiguration = RedisConfiguration.Value;
                _redisConnectionPoolManager = new RedisConnectionPoolManager(_redisConfiguration);
                _redisClient = new RedisClient(_redisConnectionPoolManager, new NewtonsoftSerializer(), _redisConfiguration);
            }   
            catch (Exception ex)
            {
                throw new Exception("An error has occurred accessing redis. See inner exception for more detail.", ex.InnerException);
            }
        }

        public async Task<bool> Add<TObject>(string key, TObject obj, int expirationDuration)
        {
            try
            {
                return await _redisClient.Db0.AddAsync(key, obj, DateTimeOffset.Now.AddMinutes(expirationDuration));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TObject> Retrieve<TObject>(string key)
        {
            try
            {
                return await _redisClient.Db0.GetAsync<TObject>(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(string key)
        {
            try
            {
                return await _redisClient.Db0.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Dispose()
        {
            _redisConnectionPoolManager.Dispose();
        }
    }
}
