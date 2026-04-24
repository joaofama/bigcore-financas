using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Financas.Infrastructure.Context;

public class RedisContext
{
    // O Lazy garante que a conexão só é aberta no primeiro pedido de base de dados
    private readonly Lazy<IConnectionMultiplexer> _connection;

    public RedisContext(IOptions<RedisSettings> settings)
    {
        _connection = new Lazy<IConnectionMultiplexer>(() =>
        {
            // Conecta usando a String de Conexão dinâmica do .env
            return ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
        });
    }

    /// <summary>
    /// Obtém a instância da base de dados do Redis
    /// </summary>
    public IDatabase GetDatabase()
    {
        return _connection.Value.GetDatabase();
    }

    /// <summary>
    /// Útil caso precises de verificar o estado da ligação ou limpar o cache
    /// </summary>
    public IServer GetServer()
    {
        var endpoint = _connection.Value.GetEndPoints().First();
        return _connection.Value.GetServer(endpoint);
    }
}