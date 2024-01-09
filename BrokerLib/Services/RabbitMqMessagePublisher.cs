using System.Text;
using System.Text.Json;
using BrokerLib.Abstract;
using RabbitMQ.Client;

namespace BrokerLib.Services;

public class RabbitMqMessagePublisher<T>: IMessagePublisher<T>, IDisposable
    where T : class
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    
    public RabbitMqMessagePublisher(string queueName, string host="localhost")
    {
        ConnectionFactory factory = new()
        {
            HostName = host
        };
        _queueName = queueName;
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
    
    public Task PublishAsync(T message)
    {
        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);
        
        _channel.BasicPublish(exchange: string.Empty, routingKey: _queueName, basicProperties: null, body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}