namespace AuthService.Settings;

public class InstanceSettings
{
    public Guid Id { get; init; } 
    public string Url { get; init; } = null!;
    public int Port { get; init; }
}