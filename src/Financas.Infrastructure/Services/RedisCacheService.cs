using System.Text.Json;
using Financas.Domain.Interfaces.Services;
using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Financas.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly RedisSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, IOptions<RedisSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;

        // Configuração para garantir que o JSON ignore diferenças entre CamelCase e PascalCase
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Recupera um objeto do cache. Se houver falha no Redis, retorna null (Fail-soft).
    /// </summary>
    public async Task<T?> ObterAsync<T>(string chave)
    {
        try
        {
            var dadoString = await _cache.GetStringAsync(chave);

            if (string.IsNullOrEmpty(dadoString))
                return default;

            return JsonSerializer.Deserialize<T>(dadoString, _jsonOptions);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// Salva um objeto no cache usando o tempo de expiração padrão das variáveis de ambiente.
    /// </summary>
    public async Task DefinirAsync<T>(string chave, T valor, TimeSpan? expiracao = null)
    {
        try
        {
            var opcoesCache = new DistributedCacheEntryOptions
            {
                // Prioriza o tempo passado por parâmetro; se nulo, usa a variável de ambiente
                AbsoluteExpirationRelativeToNow = expiracao ?? TimeSpan.FromHours(_settings.CacheExpirationInHours)
            };

            var stringJson = JsonSerializer.Serialize(valor, _jsonOptions);
            await _cache.SetStringAsync(chave, stringJson, opcoesCache);
        }
        catch (Exception)
        {            
        }
    }

    /// <summary>
    /// Remove uma chave específica do cache (Invalidação).
    /// </summary>
    public async Task RemoverAsync(string chave)
    {
        try
        {
            await _cache.RemoveAsync(chave);
        }
        catch (Exception)
        {
        }
    }
}