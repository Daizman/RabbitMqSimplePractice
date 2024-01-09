using System.Text;
using System.Text.Json;
using BrokerLib.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BrokerLib.Services;

public class RabbitMqMessageReceiver<T>: IMessageReceiver<T>, IDisposable
    where T : class
{
    private readonly List<Func<T, Task>> _handlers = new();
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;
    
    public RabbitMqMessageReceiver(string queueName, string host="localhost")
    {
        ConnectionFactory factory = new()
        {
            HostName = host
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _consumer = new EventingBasicConsumer(_channel);
        
        _consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(jsonMessage);
            
            foreach (var handler in _handlers)
            {
                if (message != null) 
                    await handler(message);
            }
        };
        
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: _consumer);
    }
    
    public void AddHandler(Func<T, Task> handler)
    {
        _handlers.Add(handler);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}