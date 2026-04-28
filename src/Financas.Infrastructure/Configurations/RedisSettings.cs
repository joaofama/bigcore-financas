namespace Financas.Infrastructure.Configurations;

public class RedisSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Password { get; set; } = string.Empty;
    public int CacheExpirationInHours { get; set; }

    public string InstanceName { get; set; } = string.Empty;

    public string ConnectionString => $"{Host}:{Port},password={Password}";
}