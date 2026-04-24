using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class RemoverCategoriaHandler : IRequestHandler<RemoverCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;
    public RemoverCategoriaHandler(ICategoriaRepository repository) => _repository = repository;

    public async Task Handle(RemoverCategoriaCommand request, CancellationToken ct)
    {        
        await _repository.RemoverAsync(request.Id);
    }
}