using DotNetEnv;
using Financas.Application;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using Financas.Infrastructure.Configurations;
using Financas.Infrastructure.Context;
using Financas.Infrastructure.Repositories;
using Financas.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Carregamento de Configurações ---
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
builder.Services.AddApplication();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<RedisContext>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// --- 5. Configuração do Swagger (Substituindo o Scalar/OpenApi nativo) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Finanças API", Version = "v1" });

    // Configuração do botão "Authorize" para JWT
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
var key = Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? throw new InvalidOperationException("JWT Secret não configurado."));

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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 7. Pipeline de Execução (Middlewares) ---
if (app.Environment.IsDevelopment())
{
    // Ativa o Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finanças API v1");
    });
}

app.UseHttpsRedirection();

// Importante: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();