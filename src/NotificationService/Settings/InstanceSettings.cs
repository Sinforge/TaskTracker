namespace NotificationService.Settings;

public class InstanceSettings
{
    public Guid Id { get; init; } 
    public string Url { get; init; } = null!;
    public int Http1Port { get; init; }
    public int GrpcPort { get; init; }
}