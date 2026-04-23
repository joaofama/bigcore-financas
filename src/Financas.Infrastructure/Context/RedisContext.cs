using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Financas.Infrastructure.Context;

public class RedisContext
{
    private readonly IConnectionMultiplexer _redis;

    public RedisContext(IOptions<RedisSettings> settings)
    {
        _redis = ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
    }

    public IDatabase GetDatabase() => _redis.GetDatabase();
}