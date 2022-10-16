using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Queue.RabbitMQ
{
    internal class RabbitMqPublisher<TMessage>
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _topicName;
        private readonly IConnection _connection;
        private readonly ILogger _logger;
        private IModel _model;

        public RabbitMqPublisher(string topicName, ILogger logger)
        {
            _logger = logger;
            _topicName = topicName;

            _connectionFactory = new ConnectionFactory
            {
                VirtualHost = "/",
                HostName = "guest",
                UserName = "guest",
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                Port = 5671,
                AutomaticRecoveryEnabled = true
            };

            _connection = _connectionFactory.CreateConnection();

            DeclareExchange();

        }

        public async Task PublishAsync(TMessage eventMessage, IDictionary<string, object> customHeaders = null)
        {
            await Task.Run(() =>
            {
                _logger.Information($"Sending message {eventMessage}");
                var messageProperties = _model.CreateBasicProperties();
                messageProperties.Persistent = false;
                messageProperties.Expiration = "30000";
                messageProperties.Headers = customHeaders;

                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage));

                _model.BasicPublish(_topicName, string.Empty, messageProperties, messageBytes);
                _logger.Information($"Message {eventMessage} sent");
            });
        }

        private void DeclareExchange()
        {
            _model?.Dispose();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_topicName, "direct", false, false, null);
        }

        public void Dispose()
        {
            if (_model != null && _model.IsOpen)
                _model.Close();

            if (_connection != null && _connection.IsOpen)
                _connection.Close();
        }
    }
}
