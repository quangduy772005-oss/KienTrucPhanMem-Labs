using System;
using System.Text;
using System.Text.Json;
using EDA.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EDA
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly string _exchangeName;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private bool _disposed;

        public RabbitMQEventBus(string hostName = "localhost", string exchangeName = "eda.events", int port = 5672, string userName = "guest", string password = "guest")
        {
            _exchangeName = exchangeName;
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
                DispatchConsumersAsync = false
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Khởi tạo Exchange loại 'topic' để phân phối event theo routing key (tên Event)
                _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false);
                Console.WriteLine($"[RabbitMQEventBus] Kết nối RabbitMQ thành công tại {hostName}:{port}, Exchange: {_exchangeName}");
            }
            catch (BrokerUnreachableException ex)
            {
                throw new InvalidOperationException(
                    $"[RabbitMQEventBus] Không thể kết nối tới RabbitMQ Server tại {hostName}:{port}.\n" +
                    $"Gợi ý: Hãy bật Docker và chạy lệnh 'docker compose up -d' hoặc khởi động dịch vụ RabbitMQ.\n" +
                    $"Chi tiết lỗi: {ex.Message}", ex);
            }
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMQEventBus));

            var routingKey = typeof(T).Name;
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // Persistent message
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            // In log hỗ trợ theo dõi
            // Console.WriteLine($"[RabbitMQEventBus] Đã publish event '{routingKey}' lên RabbitMQ: {json}");
        }

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMQEventBus));

            var routingKey = typeof(T).Name;

            // Mỗi subscriber tạo 1 queue riêng biệt (exclusive, auto-delete) để nhận bản sao event (mô hình Pub/Sub)
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var @event = JsonSerializer.Deserialize<T>(json);

                    if (@event != null)
                    {
                        handler(@event);
                    }

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMQEventBus] Lỗi khi xử lý event '{routingKey}': {ex.Message}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine($"[RabbitMQEventBus] Đã đăng ký lắng nghe event '{routingKey}' qua Queue: {queueName}");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _channel?.Close();
                _channel?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
                Console.WriteLine("[RabbitMQEventBus] Đã đóng kết nối RabbitMQ.");
            }
        }
    }
}
