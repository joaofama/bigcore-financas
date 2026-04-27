using DotNetEnv;
using Financas.API.Middlewares;
using Financas.Application;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using Financas.Infrastructure.Configurations;
using Financas.Infrastructure.Context;
using Financas.Infrastructure.Hubs;
using Financas.Infrastructure.Repositories;
using Financas.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using StackExchange.Redis;
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

var redisSettings = builder.Configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();

// --- 4. CORS  ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 5. Injeção de Dependência (Application & Infrastructure) ---
builder.Services.AddApplication();
builder.Services.AddSingleton<MongoDbContext>();

// Configuração do Redis (Cache)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisSettings?.ConnectionString;
    options.InstanceName = "Financas_";
});

// Registro de Serviços de Cache e Token
builder.Services.AddSingleton<RedisContext>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// --- CONFIGURAÇÃO SIGNALR COM REDIS BACKPLANE ---
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisSettings?.ConnectionString ?? throw new InvalidOperationException("Redis não configurado"), options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal("Financas_SignalR");
    });

builder.Services.AddScoped<INotificationService, SignalRService>();

// Registro de Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// --- 6. Configuração do Swagger com Suporte a JWT ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Financas API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer.",
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

// --- 7. Configuração de Autenticação JWT ---
var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? throw new InvalidOperationException("JWT Secret não configurado"));

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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 8. Pipeline de Execução (Middlewares) ---

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finanças API v1");
});


// O Cors deve vir antes da Autenticação e do MapHub
app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Mapeamento do Hub 
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();