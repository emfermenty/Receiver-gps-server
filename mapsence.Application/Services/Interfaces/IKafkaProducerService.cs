namespace mapsenceto.Application.Services;

public interface IKafkaProducerService
{
    Task SendMessageAsync(string message);
}