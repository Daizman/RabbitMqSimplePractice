namespace BrokerLib.Abstract;

public interface IMessagePublisher<T>
{
    Task PublishAsync(T message);
}
