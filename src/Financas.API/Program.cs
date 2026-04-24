using DotNetEnv;
using Financas.Application;
using Financas.Domain.Interfaces;
using Financas.Infrastructure.Configurations;
using Financas.Infrastructure.Context;
using Financas.Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Carrega as variáveis do arquivo .env
Env.Load();

// 2. Variáveis de Ambiente
builder.Configuration.Sources.Clear();
builder.Configuration.AddEnvironmentVariables();

// 3. Configuração Global de GUID para o MongoDB
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// 4. Mapeia classes de configuração do arquivo ENV/JSON para objetos C#
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection(nameof(RedisSettings)));

// 5. Injeção da Camada de Application (CQRS, MediatR, etc)
// Este método registra todos os Handlers automaticamente
builder.Services.AddApplication();

// 6. Injeção da Camada de Infrastructure (Contextos e Repositórios)
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<RedisContext>();

// Registro dos Repositórios
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// 7. Serviços essenciais da API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 8. Pipeline de execução (Middlewares)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();