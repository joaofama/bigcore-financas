using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services; // Importar o ICacheService
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class AtualizarCategoriaCommandHandler : IRequestHandler<AtualizarCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;
    private readonly ICacheService _cache;

    public AtualizarCategoriaCommandHandler(ICategoriaRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Handle(AtualizarCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await _repository.ObterPorIdAsync(request.Id, request.UsuarioId);
        if (categoria == null) return;

        var tipoEnum = request.Tipo.ToUpper();

        categoria.Atualizar(
            request.Nome,
            request.Icone,
            tipoEnum,
            request.CategoriaPaiId
        );

        // 1. Atualiza no MongoDB
        await _repository.AtualizarAsync(categoria);

        // 2. Limpa o cache do Redis
        var cacheKey = $"categorias:{request.UsuarioId}";
        await _cache.RemoverAsync(cacheKey);
    }
}