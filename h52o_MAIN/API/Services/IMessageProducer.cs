namespace API.Services;

public interface IMessageProducer
{
    Task SendStartCommand(int deviceId, int setId);
    Task SendStopCommand(int deviceId, int setId); 
}