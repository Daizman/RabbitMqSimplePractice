namespace BrokerLib.Abstract;

public interface IMessageReceiver<T>
    where T : class
{
    void AddHandler(Func<T, Task> handler);
}
