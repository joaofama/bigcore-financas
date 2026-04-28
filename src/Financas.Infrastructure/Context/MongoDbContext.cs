using Financas.Domain.Entities;
using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Reflection;

namespace Financas.Infrastructure.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);

    public IMongoCollection<Usuario> Usuarios => GetCollection<Usuario>("Usuarios");
    public IMongoCollection<Categoria> Categorias => GetCollection<Categoria>("Categorias");
    public IMongoCollection<Transacao> Transacoes => GetCollection<Transacao>("Transacoes");

    /// <summary>
    /// Configuração estática global para o MongoDB. 
    /// Deve ser chamada apenas uma vez na inicialização da API (Program.cs).
    /// </summary>
    public static void Configure()
    {
        RegisterConventions();
        MapEntities();
    }

    private static void RegisterConventions()
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };

        // Garante que as convenções de camelCase sejam aplicadas a todas as coleções
        ConventionRegistry.Remove("FinancasConventions");
        ConventionRegistry.Register("FinancasConventions", pack, t => true);
    }

    private static void MapEntities()
    {
        // Serializadores Padronizados para GUIDs e Decimais (essenciais para Windows/MongoDB)
        var guidSerializer = new GuidSerializer(GuidRepresentation.Standard);
        var nullableGuidSerializer = new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard));
        var decimalSerializer = new DecimalSerializer(BsonType.Decimal128);

        // --- Mapeamento USUARIO ---
        if (!BsonClassMap.IsClassMapRegistered(typeof(Usuario)))
        {
            BsonClassMap.RegisterClassMap<Usuario>(cm =>
            {
                cm.AutoMap();

                // Mapeia o construtor padrão (necessário para reconstrução do objeto)
                var ctor = typeof(Usuario).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (ctor != null) cm.MapConstructor(ctor);

                cm.MapIdProperty(c => c.Id).SetSerializer(guidSerializer);
                cm.MapMember(c => c.SaldoInicial).SetSerializer(decimalSerializer);
            });
        }

        // --- Mapeamento CATEGORIA ---
        if (!BsonClassMap.IsClassMapRegistered(typeof(Categoria)))
        {
            BsonClassMap.RegisterClassMap<Categoria>(cm =>
            {
                cm.AutoMap();

                var ctor = typeof(Categoria).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (ctor != null) cm.MapConstructor(ctor);

                cm.MapIdProperty(c => c.Id).SetSerializer(guidSerializer);
                cm.MapMember(c => c.UsuarioId).SetSerializer(guidSerializer);
                cm.MapMember(c => c.CategoriaPaiId).SetSerializer(nullableGuidSerializer);
            });
        }

        // --- Mapeamento TRANSACAO ---
        if (!BsonClassMap.IsClassMapRegistered(typeof(Transacao)))
        {
            BsonClassMap.RegisterClassMap<Transacao>(cm =>
            {
                cm.AutoMap();

                var ctor = typeof(Transacao).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (ctor != null) cm.MapConstructor(ctor);

                cm.MapIdProperty(c => c.Id).SetSerializer(guidSerializer);
                cm.MapMember(c => c.UsuarioId).SetSerializer(guidSerializer);
                cm.MapMember(c => c.CategoriaId).SetSerializer(guidSerializer);
                cm.MapMember(c => c.CategoriaPaiId).SetSerializer(nullableGuidSerializer);
                cm.MapMember(c => c.Valor).SetSerializer(decimalSerializer);
                cm.MapMember(c => c.Descricao).SetIgnoreIfNull(false);
            });
        }
    }
}