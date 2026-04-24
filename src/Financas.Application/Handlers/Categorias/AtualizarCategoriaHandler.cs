using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class AtualizarCategoriaHandler : IRequestHandler<AtualizarCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;
    public AtualizarCategoriaHandler(ICategoriaRepository repository) => _repository = repository;

    public async Task Handle(AtualizarCategoriaCommand request, CancellationToken cancellationToken)
    {
        // 1. Busca a categoria atual no banco (Garante que existe e é do usuário)
        var categoria = await _repository.ObterPorIdAsync(request.Id, request.UsuarioId);

        if (categoria == null)
            return; // Ou lança uma Exception de "Não Encontrado"

        // 2. Atualiza APENAS Nome e Icone. 
        // O campo 'Tipo' que está na entidade 'categoria' permanece intocado.
        categoria.Atualizar(request.Nome, request.Icone);

        // 3. Salva a entidade de volta no banco
        await _repository.AtualizarAsync(categoria);
    }
}