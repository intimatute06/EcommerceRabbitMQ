using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

public class RabbitMQConnection
{
    private readonly RabbitMQSettings _settings;

    public RabbitMQConnection(IConfiguration configuration)
    {
        _settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
            ?? throw new Exception("No se encontro configuracion de RabbitMQ en appsettings.json");
    }

    public async Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.Username,
            Password = _settings.Password,
            Port = _settings.Port
        };

        return await factory.CreateConnectionAsync();
    }
}