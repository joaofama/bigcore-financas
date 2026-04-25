using Financas.Application.Commands.Categorias;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class CriarCategoriaCommandHandler : IRequestHandler<CriarCategoriaCommand, Guid>
{
    private readonly ICategoriaRepository _repository;

    public CriarCategoriaCommandHandler(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CriarCategoriaCommand request, CancellationToken cancellationToken)
    {
        // 1. Instanciamos a categoria. 
        // O construtor da Entidade agora cuida de colocar o Tipo em Maiúsculo
        // e aceita o CategoriaPaiId (seja ele nulo ou não).
        var categoria = new Categoria(
            request.UsuarioId,
            request.Nome,
            request.Tipo,
            request.Icone,
            request.CategoriaPaiId
        );

        // 2. Persistimos no MongoDB
        await _repository.AdicionarAsync(categoria);

        // 3. Retornamos o ID para o Front-end
        return categoria.Id;
    }
}