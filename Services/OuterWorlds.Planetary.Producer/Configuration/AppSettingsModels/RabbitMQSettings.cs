using Newtonsoft.Json;

namespace Configuration.AppSettingsModels
{
    public class RabbitMQSettings
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("virtualHost")]
        public string VirtualHost { get; set; }

        [JsonProperty("requestHeartBeatInSeconds")]
        public ushort RequestHeartBeatInSeconds { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("queue")]
        public string Queue { get; set; }

        [JsonProperty("prefetchCount")]
        public ushort PrefetchCount { get; set; }

        [JsonProperty("prefetch")]
        public ushort Prefetch { get; set; }

        [JsonProperty("global")]
        public bool Global { get; set; }

        [JsonProperty("autoAck")]
        public bool AutoAck { get; set; }

        [JsonProperty("connectionName")]
        public string ConnectionName { get; set; }
    }
}
