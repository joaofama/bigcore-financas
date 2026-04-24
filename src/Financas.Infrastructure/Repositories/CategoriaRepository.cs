using Financas.Domain.Entities;
using Financas.Domain.Interfaces;
using Financas.Infrastructure.Context;
using MongoDB.Driver;

namespace Financas.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly IMongoCollection<Categoria> _categorias;

    public CategoriaRepository(MongoDbContext contexto)
    {
        _categorias = contexto.Categorias;
    }

    public async Task<IEnumerable<Categoria>> ObterTodasAsync()
    {
        return await _categorias.Find(_ => true).ToListAsync();
    }

    public async Task<Categoria?> ObterPorIdAsync(Guid id)
    {
        return await _categorias.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Categoria?> ObterPorNomeAsync(string nome)
    {
        return await _categorias.Find(x => x.Nome == nome).FirstOrDefaultAsync();
    }

    public async Task AdicionarAsync(Categoria categoria)
    {
        await _categorias.InsertOneAsync(categoria);
    }

    public async Task AtualizarAsync(Guid id, Categoria categoria)
    {
        await _categorias.ReplaceOneAsync(x => x.Id == id, categoria);
    }

    public async Task RemoverAsync(Guid id)
    {
        await _categorias.DeleteOneAsync(x => x.Id == id);
    }
}