public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string exchange, string routingKey);
}
