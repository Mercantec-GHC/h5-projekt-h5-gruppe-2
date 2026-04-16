using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace API.Services;

public class RabbitMQProducer : IMessageProducer
{
    private readonly IConfiguration _config;

    public RabbitMQProducer(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendStartCommand(int deviceId, int setId)
    {
        await SendCommand(deviceId, setId, "START");
    }

    public async Task SendStopCommand(int deviceId, int setId)
    {
        await SendCommand(deviceId, setId, "STOP");
    }

    private async Task SendCommand(int deviceId, int setId, string action)
    {
        var factory = new ConnectionFactory { HostName = _config["RabbitMQ:Host"] ?? "localhost" };
        
        // I v7+ skal forbindelser og kanaler oprettes asynkront
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Deklarerer køen "device_commands", fælles kanal til Python-workeren
        await channel.QueueDeclareAsync(
            queue: "device_commands", 
            durable: true, 
            exclusive: false, 
            autoDelete: false);

        var message = new 
        { 
            DeviceId = deviceId, 
            MeasurementSetId = setId, 
            Action = action, // "START" eller "STOP"
            Timestamp = DateTime.UtcNow 
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        // Publicerer beskeden direkte til køen (Default Exchange)
        await channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: "device_commands", 
            body: body);
    }
}