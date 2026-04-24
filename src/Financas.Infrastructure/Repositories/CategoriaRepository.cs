using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Infrastructure.Context;
using MongoDB.Driver;

namespace Financas.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly IMongoCollection<Categoria> _categorias;

    public CategoriaRepository(MongoDbContext context)
    {
        // Obtém a coleção de categorias do contexto do MongoDB
        _categorias = context.GetCollection<Categoria>("Categorias");
    }

    public async Task<IEnumerable<Categoria>> ObterTodasPorUsuarioAsync(Guid usuarioId)
    {
        // Filtra apenas as categorias onde o UsuarioId coincide com o logado
        return await _categorias
            .Find(c => c.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<Categoria?> ObterPorIdAsync(Guid id, Guid usuarioId)
    {
        // Filtro duplo: ID do documento E ID do dono
        return await _categorias
            .Find(c => c.Id == id && c.UsuarioId == usuarioId)
            .FirstOrDefaultAsync();
    }

    public async Task<Categoria?> ObterPorNomeAsync(string nome, Guid usuarioId)
    {
        // Busca por nome dentro do escopo do usuário (evita duplicidade pessoal)
        return await _categorias
            .Find(c => c.Nome == nome && c.UsuarioId == usuarioId)
            .FirstOrDefaultAsync();
    }

    public async Task AdicionarAsync(Categoria categoria)
    {
        // A entidade já deve chegar aqui com o UsuarioId preenchido pelo Handler
        await _categorias.InsertOneAsync(categoria);
    }

    public async Task AtualizarAsync(Categoria categoria)
    {
        // O ReplaceOne garante que só atualiza se o ID e o UsuarioId baterem no banco
        await _categorias.ReplaceOneAsync(
            c => c.Id == categoria.Id && c.UsuarioId == categoria.UsuarioId,
            categoria
        );
    }

    public async Task RemoverAsync(Guid id, Guid usuarioId)
    {
        // Exclusão segura: impossível deletar categoria de terceiros
        await _categorias.DeleteOneAsync(c => c.Id == id && c.UsuarioId == usuarioId);
    }
}