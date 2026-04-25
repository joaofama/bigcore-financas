using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class RemoverCategoriaCommandHandler : IRequestHandler<RemoverCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;

    public RemoverCategoriaCommandHandler(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemoverCategoriaCommand request, CancellationToken ct)
    {
        // Execução pura e simples
        await _repository.RemoverAsync(request.Id, request.UsuarioId);
    }
}