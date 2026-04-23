using Financas.Domain.Entities;
using Financas.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Financas.Infrastructure.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        // 1. Configura o cliente usando a ConnectionString montada na classe de Settings
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        // 2. Regista as convenções e mapeamentos
        RegisterConventions();
        MapEntities();
    }

    // Acesso às Coleções (Equivalente às Tabelas no SQL)
    public IMongoCollection<Usuario> Usuarios => _database.GetCollection<Usuario>("Usuarios");
    public IMongoCollection<Categoria> Categorias => _database.GetCollection<Categoria>("Categorias");
    public IMongoCollection<Transacao> Transacoes => _database.GetCollection<Transacao>("Transacoes");

    private void RegisterConventions()
    {
        // Define que o JSON no MongoDB usará camelCase (ex: NomeUsuario -> nomeUsuario)
        // E ignora campos extras que possam existir no banco mas não na classe
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("FinancasConventions", pack, t => true);
    }

    private void MapEntities()
    {
        // Configuração Global para GUIDs: Salva como String no banco para ser legível
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

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
                // Garante que a lista de subcategorias seja mapeada corretamente
                cm.MapField(c => c.Subcategorias).SetElementName("subcategorias");
            });
        }

        // Mapeamento da Entidade SUBCATEGORIA (Como é usada dentro da Categoria)
        if (!BsonClassMap.IsClassMapRegistered(typeof(Subcategoria)))
        {
            BsonClassMap.RegisterClassMap<Subcategoria>(cm => { cm.AutoMap(); });
        }

        // Mapeamento da Entidade TRANSACAO
        if (!BsonClassMap.IsClassMapRegistered(typeof(Transacao)))
        {
            BsonClassMap.RegisterClassMap<Transacao>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                // Mapeia os Value Objects (Records) que estão dentro da transação
                cm.MapProperty(c => c.Categoria).SetElementName("categoria");
                cm.MapProperty(c => c.Subcategoria).SetElementName("subcategoria");
            });
        }
    }
}