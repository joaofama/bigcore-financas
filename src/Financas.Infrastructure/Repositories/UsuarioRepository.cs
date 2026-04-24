using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Infrastructure.Context;
using MongoDB.Driver;

namespace Financas.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _usuarios;

    public UsuarioRepository(MongoDbContext context)
    {
        // Obtém a coleção "Usuarios" do seu contexto MongoDB
        // O nome da coleção deve ser o mesmo usado no seu script de seed.js
        _usuarios = context.GetCollection<Usuario>("Usuarios");
    }

    /// <summary>
    /// Busca um usuário ativo no banco de dados pela combinação de e-mail e senha.
    /// </summary>
    public async Task<Usuario?> ObterPorCredenciaisAsync(string email, string senha)
    {
        // Criamos o filtro para buscar exatamente o usuário de teste
        // u.Ativo garante que não pegamos usuários desativados
        var filter = Builders<Usuario>.Filter.And(
            Builders<Usuario>.Filter.Eq(u => u.Email, email),
            Builders<Usuario>.Filter.Eq(u => u.Senha, senha),
            Builders<Usuario>.Filter.Eq(u => u.Ativo, true)
        );

        // Executa a busca de forma assíncrona no MongoDB
        return await _usuarios.Find(filter).FirstOrDefaultAsync();
    }
}