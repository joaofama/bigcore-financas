using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services; // Importar o ICacheService
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class RemoverCategoriaCommandHandler : IRequestHandler<RemoverCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;
    private readonly ICacheService _cache;

    public RemoverCategoriaCommandHandler(ICategoriaRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Handle(RemoverCategoriaCommand request, CancellationToken ct)
    {
        // 1. Remove do MongoDB
        await _repository.RemoverAsync(request.Id, request.UsuarioId);

        // 2. Limpa o cache do Redis para o usuário
        var cacheKey = $"categorias:{request.UsuarioId}";
        await _cache.RemoverAsync(cacheKey);
    }
}