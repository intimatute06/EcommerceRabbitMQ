public interface IMessageConsumer
{
    Task ConsumeAsync(string queue, Func<string, Task> onMessageReceived);
}