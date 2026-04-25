using DotNetEnv;
using Financas.Application;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using Financas.Infrastructure.Configurations;
using Financas.Infrastructure.Context;
using Financas.Infrastructure.Repositories;
using Financas.Infrastructure.Services;
using Financas.API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Carregamento de Configurações (.env e Variáveis de Ambiente) ---
Env.Load();
builder.Configuration.Sources.Clear();
builder.Configuration.AddEnvironmentVariables();

// --- 2. Configurações Globais MongoDB ---
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// --- 3. Mapeamento de Configurações (Options Pattern) ---
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

// --- 4. Injeção de Dependência (Application & Infrastructure) ---
builder.Services.AddApplication(); // MediatR, AutoMapper, Validators
builder.Services.AddSingleton<MongoDbContext>();

// Configuração do Redis Nativo (IDistributedCache)
var redisSettings = builder.Configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisSettings?.ConnectionString;
    options.InstanceName = "Financas_";
});

// Registro do Contexto e Serviços de Cache
builder.Services.AddSingleton<RedisContext>(); // Contexto para acesso direto se necessário
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Registro de Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// --- 5. Configuração do Swagger com Suporte a JWT ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Finanças API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- 6. Configuração de Autenticação JWT ---
var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? throw new InvalidOperationException("JWT Secret não configurado no .env"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 7. Pipeline de Execução (Middlewares) ---

// Middleware Global de Exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finanças API v1");
    });
}

app.UseHttpsRedirection();

// Ordem obrigatória: Autenticação antes de Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();