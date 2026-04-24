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

    public async Task Handle(RemoverCategoriaCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscamos a categoria garantindo que ela pertence ao usuário
        var categoria = await _repository.ObterPorIdAsync(request.Id, request.UsuarioId);

        // 2. Validação de segurança: Se não existir ou não for do usuário, não fazemos nada
        // (Ou você pode lançar uma Exception customizada aqui)
        if (categoria == null)
            return;

        // 3. Executamos a remoção (ou inativação) no banco
        await _repository.RemoverAsync(request.Id, request.UsuarioId);
    }
}