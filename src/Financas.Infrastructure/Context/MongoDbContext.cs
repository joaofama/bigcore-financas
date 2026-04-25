using Financas.Domain.Entities;
using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Financas.Infrastructure.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        RegisterConventions();
        MapEntities();
    }

    /// <summary>
    /// Método genérico para obter qualquer coleção
    /// </summary>
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

    // Coleções 
    public IMongoCollection<Usuario> Usuarios => GetCollection<Usuario>("Usuarios");
    public IMongoCollection<Categoria> Categorias => GetCollection<Categoria>("Categorias");
    public IMongoCollection<Transacao> Transacoes => GetCollection<Transacao>("Transacoes");

    private static void RegisterConventions()
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };

        // Verifica se já está registrado para evitar erros em Hot Reload
        if (ConventionRegistry.Lookup(typeof(CamelCaseElementNameConvention)) == null)
        {
            ConventionRegistry.Register("FinancasConventions", pack, t => true);
        }
    }

    private static void MapEntities()
    {
        // Mapeamento da Entidade USUARIO
        if (!BsonClassMap.IsClassMapRegistered(typeof(Usuario)))
        {
            BsonClassMap.RegisterClassMap<Usuario>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                cm.SetIgnoreExtraElements(true);
            });
        }

        // Mapeamento da Entidade CATEGORIA
        if (!BsonClassMap.IsClassMapRegistered(typeof(Categoria)))
        {
            BsonClassMap.RegisterClassMap<Categoria>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                // Removido o mapeamento de Subcategorias pois adotamos o modelo Flat
            });
        }

        // Mapeamento da Entidade TRANSACAO
        if (!BsonClassMap.IsClassMapRegistered(typeof(Transacao)))
        {
            BsonClassMap.RegisterClassMap<Transacao>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                // Removidos os mapeamentos de Categoria e Subcategoria aninhados.
                // Agora ela usa apenas propriedades nativas (CategoriaId) que o AutoMap resolve sozinho.
            });
        }
    }
}