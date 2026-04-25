using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Financas.Infrastructure.Context;

public class RedisContext
{
    private readonly RedisSettings _settings;
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisContext(IOptions<RedisSettings> settings)
    {
        _settings = settings.Value;
        _connection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(_settings.ConnectionString));
    }

    public IDatabase GetDatabase() => _connection.Value.GetDatabase();
}