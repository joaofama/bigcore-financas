using DotNetEnv;
using Financas.Infrastructure.Configurations;
using Financas.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. Carrega o arquivo .env
Env.Load();

// 2. Limpa provedores padrão e foca 100% em Variáveis de Ambiente
builder.Configuration.Sources.Clear();
builder.Configuration.AddEnvironmentVariables();

// 3. Mapeia classes de configuração
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection(nameof(RedisSettings)));

// 4. Injeção do Contexto do MongoDB (Singleton)
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<RedisContext>();

// 5. Serviços da API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 6. Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();